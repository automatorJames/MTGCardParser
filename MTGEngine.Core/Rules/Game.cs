namespace MTGEngine.Core;

/// <summary>A player's starting deck.</summary>
public sealed record Deck(string PlayerName, ImmutableList<CardDefinition> Cards);

/// <summary>
/// The rules kernel: turns, steps, priority, playing lands, casting spells, activating abilities, resolving the
/// stack, combat, state-based actions and triggers. Decides which <see cref="GameEvent"/>s happen; the state only
/// changes through <see cref="Emit"/>. Players' choices come from their <see cref="IDecisionMaker"/>s as data.
/// </summary>
public sealed partial class Game
{
    readonly IDecisionMaker[] _agents;
    readonly List<GameEvent> _log = [];
    readonly List<(GameEvent Event, GameState Before, GameState After)> _pending = [];
    PlayerId _defender;
    bool _attackersThisCombat;

    public GameState State { get; private set; }
    public IReadOnlyList<GameEvent> Log => _log;
    public Random Rng { get; }
    public int Seed { get; }
    public Queue<PlayerId> ExtraTurns { get; } = new();
    public bool LastOptionalTaken { get; set; }

    /// <summary>Whether the last condition an effect checked held, for the "otherwise" after it.</summary>
    public bool LastConditionHeld { get; set; }

    /// <summary>Whether the last library search an effect offered happened, for "if you search your library this way".</summary>
    public bool SearchedThisWay { get; set; }
    public Dictionary<string, int> Skips { get; } = new();
    public int MaxHandSize { get; init; } = 7;

    Game(GameState state, IDecisionMaker[] agents, int seed)
    {
        State = state;
        _agents = agents;
        Seed = seed;
        Rng = new Random(seed);
    }

    public static Game Start(IReadOnlyList<Deck> decks, IReadOnlyList<IDecisionMaker> agents, int seed = 0, int startingLife = 20, int handSize = 7, bool shuffle = true)
    {
        if (decks.Count != agents.Count || decks.Count < 2)
            throw new ArgumentException("A game needs at least two decks, one agent each.");

        var players = decks.Select((deck, i) => new PlayerState(new PlayerId(i), deck.PlayerName, startingLife,
            ImmutableList<ObjectId>.Empty, ImmutableList<ObjectId>.Empty, ImmutableList<ObjectId>.Empty, ImmutableList<ObjectId>.Empty, ManaPool.Empty)).ToImmutableArray();

        var state = new GameState(players, ImmutableDictionary<ObjectId, GameObject>.Empty, ImmutableList<StackItem>.Empty, 0, new PlayerId(0), Step.Cleanup, new PlayerId(0),
            ImmutableList<TemporaryEffect>.Empty, 1, 1);
        var game = new Game(state, agents.ToArray(), seed);

        game.Emit(new GameStarted(seed, decks.Select(x => x.PlayerName).ToImmutableArray()));

        for (var p = 0; p < decks.Count; p++)
            foreach (var card in decks[p].Cards)
            {
                var id = new ObjectId(game.State.NextObjectId);
                game.Emit(new ObjectCreated(new GameObject(id, card, new PlayerId(p), new PlayerId(p), Zone.Library)));
            }

        if (shuffle)
            foreach (var player in game.State.PlayerIds)
                game.Shuffle(player);

        foreach (var player in game.State.PlayerIds)
            game.Draw(player, handSize);

        game._pending.Clear();
        return game;
    }

    public void Emit(GameEvent evt)
    {
        var before = State;
        State = Reducer.Apply(State, evt);
        _log.Add(evt);
        _pending.Add((evt, before, State));
    }

    public DecisionResponse Decide(Decision decision)
    {
        var response = _agents[decision.Player.Value].Decide(State, decision) ?? new DecisionResponse();
        return response;
    }

    /// <summary>Notes an IR node the engine couldn't act on, and goes on.</summary>
    public void Skip(IrNode node, string why)
    {
        var kind = node?.GetType().Name ?? "null";
        Skips[kind] = Skips.GetValueOrDefault(kind) + 1;
        Emit(new EffectSkipped(kind, why));
    }

    // ---- Running the game -------------------------------------------------------------------------------------

    public void Run(int maxTurns = 100)
    {
        while (!State.Over)
        {
            if (State.Turn >= maxTurns)
            {
                Emit(new GameEnded(null, "turn limit"));
                return;
            }

            var next = ExtraTurns.Count > 0 ? ExtraTurns.Dequeue()
                : State.Turn == 0 ? new PlayerId(0)
                : State.NextInTurnOrder(State.ActivePlayer);
            PlayTurn(next);
        }
    }

    public void PlayTurn(PlayerId player)
    {
        Emit(new TurnBegan(State.Turn + 1, player));
        _attackersThisCombat = false;

        foreach (var step in Enum.GetValues<Step>())
        {
            if (State.Over)
                return;
            BeginStep(step);
        }
    }

    void BeginStep(Step step)
    {
        var active = State.ActivePlayer;

        if (step is Step.DeclareBlockers or Step.CombatDamage && !_attackersThisCombat)
            return;

        Emit(new StepBegan(step, active));

        switch (step)
        {
            case Step.Untap:
                Emit(new SummoningSicknessCleared(active));
                foreach (var obj in State.Battlefield.Where(x => x.Controller == active && x.Tapped).ToList())
                    if (!DoesntUntap(obj))
                        Emit(new Untapped(obj.Id));
                foreach (var stale in State.Temporary.Where(x => x.DoesntUntap && State.Object(x.Target)?.Controller == active).ToList())
                    Emit(new TemporaryEffectsExpired(State.Turn));
                return;

            case Step.Draw:
                if (State.Turn > 1)
                    Draw(active, 1);
                Priority();
                return;

            case Step.DeclareAttackers:
                DeclareAttackers();
                if (_attackersThisCombat)
                    Priority();
                return;

            case Step.DeclareBlockers:
                DeclareBlockers();
                Priority();
                return;

            case Step.CombatDamage:
                CombatDamage();
                Priority();
                return;

            case Step.EndCombat:
                Emit(new CombatEnded());
                Priority();
                return;

            case Step.Cleanup:
                Cleanup();
                return;

            case Step.Main1:
                foreach (var saga in State.Battlefield.Where(x => x.Controller == active && x.Card.IsSaga).ToList())
                    Emit(new CountersChanged(saga.Id, "lore", 1));
                Priority();
                return;

            default:
                Priority();
                return;
        }
    }

    bool DoesntUntap(GameObject obj) =>
        obj.Card.Abilities.Any(x => x is StaticAbility { Effect: DoesntUntap { Objects: Self, If: null } })
        || State.Temporary.Any(x => x.Target == obj.Id && x.DoesntUntap);

    void Cleanup()
    {
        var active = State.ActivePlayer;
        var hand = State.Player(active).Hand;
        if (hand.Count > MaxHandSize)
        {
            var excess = hand.Count - MaxHandSize;
            var chosen = Decide(new ChooseTargets(active, "Discard down to hand size", hand.Select(TargetRef.Of).ToImmutableList(), excess, excess)).Targets?.Where(x => x.IsObject).Select(x => x.Object.Value).Distinct().Take(excess).ToList();
            if (chosen is null || chosen.Count < excess)
                chosen = hand.Take(excess).ToList();
            foreach (var id in chosen)
                Discard(active, id);
        }

        Emit(new DamageRemoved());
        Emit(new TemporaryEffectsExpired(State.Turn));
        EmptyPools();
        CheckStateAndTriggers();
        if (!State.StackIsEmpty)
            Priority();
    }

    void EmptyPools()
    {
        foreach (var player in State.Players.Where(x => !x.Pool.IsEmpty))
            Emit(new PoolEmptied(player.Id));
    }

    // ---- Priority and the stack -------------------------------------------------------------------------------

    void Priority()
    {
        var player = State.ActivePlayer;
        var passes = 0;
        var inGame = State.PlayersInGame.Count();

        while (!State.Over)
        {
            CheckStateAndTriggers();
            if (State.Over)
                return;

            if (State.Player(player).Lost)
            {
                player = State.NextInTurnOrder(player);
                continue;
            }

            Emit(new PriorityGiven(player));
            var actions = LegalActions(player);
            var response = Decide(new ChooseAction(player, actions));
            var action = actions[Math.Clamp(response.Index, 0, actions.Count - 1)];

            if (action is PassPriority)
            {
                Emit(new Passed(player));
                passes++;
                if (passes >= inGame)
                {
                    if (State.StackIsEmpty)
                    {
                        EmptyPools();
                        return;
                    }
                    ResolveTop();
                    passes = 0;
                    player = State.ActivePlayer;
                    continue;
                }
                player = State.NextInTurnOrder(player);
                continue;
            }

            passes = 0;
            Perform(action, player, response);
        }
    }

    public ImmutableList<LegalAction> LegalActions(PlayerId player)
    {
        List<LegalAction> actions = [new PassPriority()];
        var state = State;
        var sorcerySpeed = player == state.ActivePlayer && state.IsMainPhase && state.StackIsEmpty;
        var playerState = state.Player(player);

        foreach (var id in playerState.Hand)
        {
            var obj = state.Object(id);
            if (obj is null)
                continue;
            var card = obj.Card;

            if (card.IsLand)
            {
                if (sorcerySpeed && playerState.LandsPlayedThisTurn < 1)
                    actions.Add(new PlayLand(id));
                continue;
            }

            var instantSpeed = card.IsInstant || card.Keywords.Contains("Flash");
            if (!instantSpeed && !sorcerySpeed)
                continue;
            if (!card.Abilities.OfType<ActivationLimits>().Where(x => x.ForCasting).SelectMany(x => x.Timings).All(t => TimingAllows(t, obj, player)))
                continue;
            if (!CanAffordToCast(obj, player))
                continue;
            if (!TargetsAvailable(SpellSlots(card), player, id))
                continue;
            actions.Add(new CastSpell(id, card.ManaCost.X > 0 ? ManaPayment.MaxX(state, player, CostToCast(obj, player, card.ManaCost, null)) : 0));
        }

        foreach (var (permission, obj) in Permitted(player).ToList())
        {
            var card = obj.Card;
            if (card.IsLand)
            {
                if (sorcerySpeed && playerState.LandsPlayedThisTurn < 1)
                    actions.Add(new PlayLand(obj.Id));
                continue;
            }
            if (!(card.IsInstant || card.Keywords.Contains("Flash")) && !sorcerySpeed)
                continue;
            if (!permission.Free && !CanAffordToCast(obj, player))
                continue;
            if (!TargetsAvailable(SpellSlots(card), player, obj.Id))
                continue;
            actions.Add(new CastSpell(obj.Id, !permission.Free && card.ManaCost.X > 0 ? ManaPayment.MaxX(state, player, CostToCast(obj, player, card.ManaCost, null)) : 0, permission.Free));
        }

        foreach (var obj in state.Battlefield.Where(x => x.Controller == player))
        {
            var traits = Characteristics.Of(state, obj);
            if (traits.LostAbilities)
                continue;
            var abilities = traits.Abilities;
            var limits = abilities.OfType<ActivationLimits>().SelectMany(x => x.Timings).ToList();
            var sorceryOnly = limits.Any(t => t.Kind == nameof(TimingKind.SorcerySpeed));
            var onceEachTurn = limits.Any(t => t.Kind is nameof(TimingKind.OnceEachTurn) or nameof(TimingKind.Once));
            var index = -1;
            foreach (var ability in abilities)
            {
                index++;
                switch (ability)
                {
                    case ActivatedAbility activated:
                        if (activated.Effect is AddMana)
                            continue;
                        if (sorceryOnly && !sorcerySpeed)
                            continue;
                        if (onceEachTurn && state.Activations.GetValueOrDefault($"{obj.Id}:{index}") > 0)
                            continue;
                        if (!limits.All(t => TimingAllows(t, obj, player)))
                            continue;
                        if (!CanPayCosts(obj, activated.Costs, player))
                            continue;
                        if (!TargetsAvailable(TargetSlots.Of([activated.Effect]).ToList(), player, obj.Id))
                            continue;
                        actions.Add(new ActivateAbility(obj.Id, index, Interpreter.Describe(activated.Effect)));
                        break;
                    case LoyaltyAbility loyalty:
                    {
                        if (!sorcerySpeed || state.Activations.Keys.Any(k => k.StartsWith($"{obj.Id}:")))
                            continue;
                        var change = (loyalty.Change as Fixed)?.Value ?? 0;
                        if (obj.CountersOf("loyalty") + change < 0)
                            continue;
                        if (!TargetsAvailable(TargetSlots.Of([loyalty.Effect]).ToList(), player, obj.Id))
                            continue;
                        actions.Add(new ActivateAbility(obj.Id, index, $"[{change:+#;-#;0}] {Interpreter.Describe(loyalty.Effect)}"));
                        break;
                    }
                }
            }
        }

        return actions.ToImmutableList();
    }

    /// <summary>The targets a spell asks for: its effects' target selectors, or for an aura what it enchants.</summary>
    List<TargetSlot> SpellSlots(CardDefinition card)
    {
        if (card.IsPermanentCard)
        {
            if (card.IsAura && card.Abilities.OfType<EnchantAbility>().FirstOrDefault() is { } enchant)
                return [new TargetSlot(enchant.Target is ObjectSelector objects ? objects with { Quantifier = Quantifier.Target } : enchant.Target, null, false, 1)];
            return [];
        }
        // A modal spell's targets depend on the modes chosen as it's cast, so they're checked then, not here.
        return TargetSlots.Of(card.SpellEffects.Where(x => x is not ModalEffect)).ToList();
    }

    /// <summary>A modal effect with its modes chosen by <paramref name="player"/>: the chosen modes' effects in sequence; any other effect as it is.</summary>
    (Effect Effect, ImmutableDictionary<string, ImmutableList<TargetRef>> Bindings) ResolveModes(Effect effect, PlayerId player)
    {
        if (effect is not ModalEffect modal)
            return (effect, null);

        var (min, max) = modal.Count switch
        {
            "One" or "UpToOne" => (modal.Count == "One" ? 1 : 0, 1),
            "Two" or "Both" => (2, 2),
            "Three" => (3, 3),
            "Four" => (4, 4),
            "OneOrBoth" or "UpToTwo" => (modal.Count == "OneOrBoth" ? 1 : 0, 2),
            "OneOrMore" => (1, modal.Modes.Count),
            "AnyNumber" or "UpToFour" or "UpToX" or "UpToThatMany" => (0, modal.Modes.Count),
            _ => (1, 1),
        };
        max = Math.Min(max, modal.Modes.Count);

        List<int> chosen = [];
        var options = modal.Modes.Select(m => Interpreter.Describe(m.Effect)).ToImmutableList();
        for (var i = 0; i < max; i++)
        {
            var remaining = Enumerable.Range(0, modal.Modes.Count).Where(m => !chosen.Contains(m)).ToList();
            if (remaining.Count == 0)
                break;
            if (chosen.Count >= min && i > 0 && !Decide(new YesNo(player, "Choose another mode?")).Yes)
                break;
            var pick = Decide(new ChooseOne(player, "Choose a mode", remaining.Select(m => options[m]).ToImmutableList())).Index;
            chosen.Add(remaining[Math.Clamp(pick, 0, remaining.Count - 1)]);
        }

        Emit(new ModesChosen(player, chosen.ToImmutableList()));
        var steps = chosen.Select(m => modal.Modes[m].Effect).ToList();
        return (steps.Count == 1 ? steps[0] : new Sequence(steps), null);
    }

    bool TargetsAvailable(List<TargetSlot> slots, PlayerId player, ObjectId? source) =>
        slots.All(slot => slot.UpTo || TargetSlots.Candidates(slot, this, player, source).Count > 0);

    bool CanPayCosts(GameObject source, IReadOnlyList<Cost> costs, PlayerId player)
    {
        var traits = Characteristics.Of(State, source);
        foreach (var cost in Flatten(costs))
            switch (cost)
            {
                case TapSelf:
                    if (source.Tapped || traits.IsCreature && source.EnteredThisTurn && !traits.HasKeyword("Haste"))
                        return false;
                    break;
                case UntapSelf:
                    if (!source.Tapped)
                        return false;
                    break;
                case PayMana mana:
                    if (!ManaPayment.CanPay(State, player, ManaCost.Parse(string.Concat(mana.Symbols))))
                        return false;
                    break;
                case PayLife life:
                    if (life.Amount is not Fixed amount || State.Player(player).Life < amount.Value)
                        return false;
                    break;
                case ActionCost { Action: Sacrifice sacrifice }:
                    if (sacrifice.Objects is Self ? source.Zone != Zone.Battlefield : !SacrificeCandidates(sacrifice, player, source.Id).Any())
                        return false;
                    break;
                case ActionCost { Action: Discard discard }:
                    if (State.Player(player).Hand.Count < ((discard.Count as Fixed)?.Value ?? 1))
                        return false;
                    break;
                case ActionCost { Action: RemoveCounters { Objects: null or Self } remove } when remove.Counters.Kind is not null:
                    if (source.CountersOf(remove.Counters.Kind) < ((remove.Counters.Count as Fixed)?.Value ?? 1))
                        return false;
                    break;
                case ActionCost { Action: var action }:
                    if (!ActionPayable(action, player, source.Id, 0))
                        return false;
                    break;
                default:
                    return false;
            }
        return true;
    }

    static IEnumerable<Cost> Flatten(IEnumerable<Cost> costs) =>
        costs.SelectMany(x => x is CompoundCost compound ? Flatten(compound.Parts) : [x]);

    IEnumerable<GameObject> SacrificeCandidates(Sacrifice sacrifice, PlayerId player, ObjectId source)
    {
        var ctx = new EffectContext(this, player, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
        if (sacrifice.Objects is ObjectSelector selector)
            return State.Battlefield.Where(x => x.Controller == player && Filters.Matches(State, selector.Filter, x, player, source, selector.Other ? source : null, ctx: ctx));
        return [];
    }

    void Perform(LegalAction action, PlayerId player, DecisionResponse response)
    {
        switch (action)
        {
            case PlayLand land:
                MoveToZone(land.Card, Zone.Battlefield, player);
                Emit(new LandPlayed(player, land.Card));
                return;
            case CastSpell cast:
                Cast(cast.Card, player, cast.MaxX, cast.Free);
                return;
            case ActivateAbility activate:
                Activate(activate.Source, activate.Index, player);
                return;
        }
    }

    void Activate(ObjectId source, int index, PlayerId player)
    {
        var obj = State.Object(source);
        if (obj is null)
            return;
        var found = Characteristics.AbilitiesOf(State, obj).ElementAtOrDefault(index);

        if (found is LoyaltyAbility loyalty)
        {
            var change = (loyalty.Change as Fixed)?.Value ?? 0;
            var (loyaltyEffect, loyaltyBindings) = ResolveModes(loyalty.Effect, player);
            var loyaltySlots = TargetSlots.Of([loyaltyEffect]).ToList();
            var loyaltyTargets = ChooseTargetsFor(loyaltySlots, player, source);
            if (loyaltyTargets is null)
                return;
            if (change != 0)
                Emit(new CountersChanged(source, "loyalty", change));
            var loyaltyItem = new StackItem(State.NextStackId, player, source, StackKind.Activated, [loyaltyEffect], loyaltyTargets, 0, Description: $"{obj.Card.Name} [{change:+#;-#;0}]", AbilityIndex: index, Bindings: loyaltyBindings);
            Emit(new AbilityActivated(loyaltyItem));
            return;
        }

        if (found is not ActivatedAbility ability)
            return;

        var (effect, bindings) = ResolveModes(ability.Effect, player);
        var slots = TargetSlots.Of([effect]).ToList();
        var targets = ChooseTargetsFor(slots, player, source);
        if (targets is null)
            return;

        List<TargetRef> paid = [];
        foreach (var cost in Flatten(ability.Costs))
            switch (cost)
            {
                case TapSelf:
                    Emit(new Tapped(source));
                    break;
                case UntapSelf:
                    Emit(new Untapped(source));
                    break;
                case PayMana mana:
                    if (!Pay(player, ManaCost.Parse(string.Concat(mana.Symbols))))
                        return;
                    break;
                case PayLife { Amount: Fixed life }:
                    ChangeLife(player, -life.Value, "cost");
                    break;
                case ActionCost { Action: Sacrifice { Objects: Self } }:
                    Sacrifice(player, source);
                    paid.Add(TargetRef.Of(source));
                    break;
                case ActionCost { Action: Sacrifice sacrifice }:
                {
                    var candidates = SacrificeCandidates(sacrifice, player, source).Select(x => TargetRef.Of(x.Id)).ToImmutableList();
                    var chosen = candidates.Count == 1 ? candidates : Decide(new ChooseTargets(player, "Sacrifice", candidates, 1, 1)).Targets ?? candidates.Take(1).ToImmutableList();
                    if (chosen.FirstOrDefault(x => x.IsObject) is { IsObject: true } pick)
                    {
                        Sacrifice(player, pick.Object.Value);
                        paid.Add(pick);
                    }
                    break;
                }
                case ActionCost { Action: Discard discard }:
                {
                    var count = (discard.Count as Fixed)?.Value ?? 1;
                    var hand = State.Player(player).Hand.Select(TargetRef.Of).ToImmutableList();
                    var chosen = Decide(new ChooseTargets(player, "Discard", hand, count, count)).Targets ?? hand.Take(count).ToImmutableList();
                    foreach (var card in chosen.Where(x => x.IsObject).Take(count).ToList())
                    {
                        Discard(player, card.Object.Value);
                        paid.Add(card);
                    }
                    break;
                }
                case ActionCost { Action: RemoveCounters { Objects: null or Self } remove } when remove.Counters.Kind is not null:
                    Emit(new CountersChanged(source, remove.Counters.Kind, -((remove.Counters.Count as Fixed)?.Value ?? 1)));
                    break;
                case ActionCost { Action: var action }:
                {
                    var costCtx = Interpreter.Execute(action, new EffectContext(this, player, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance)));
                    paid.AddRange(costCtx.Affected ?? costCtx.Last ?? []);
                    break;
                }
            }
        // What the costs took, for "the sacrificed creature's power", "the discarded card's mana value".
        bindings = (bindings ?? ImmutableDictionary<string, ImmutableList<TargetRef>>.Empty).SetItem("#cost", paid.ToImmutableList());

        var item = new StackItem(State.NextStackId, player, source, StackKind.Activated, [effect], targets, 0, Description: $"{obj.Card.Name}: {Interpreter.Describe(effect)}", Costs: ability.Costs.ToImmutableList(), AbilityIndex: index, Bindings: bindings);
        Emit(new AbilityActivated(item));
    }

    /// <summary>Targets for each slot, chosen by <paramref name="player"/>; null when a required slot has no legal target.</summary>
    ImmutableList<TargetRef> ChooseTargetsFor(List<TargetSlot> slots, PlayerId player, ObjectId? source)
    {
        List<TargetRef> targets = [];
        foreach (var slot in slots)
        {
            var candidates = TargetSlots.Candidates(slot, this, player, source).Except(targets).ToImmutableList();
            var max = Math.Min(slot.Count, candidates.Count);
            var min = slot.UpTo ? 0 : Math.Min(1, max);
            if (candidates.Count == 0)
            {
                if (slot.UpTo)
                    continue;
                return null;
            }
            var chosen = Decide(new ChooseTargets(player, "Choose a target", candidates, min, max)).Targets?.Where(candidates.Contains).Distinct().Take(max).ToList() ?? [];
            if (chosen.Count < min)
                chosen = candidates.Take(min).ToList();
            targets.AddRange(chosen);
        }
        return targets.ToImmutableList();
    }

    bool Pay(PlayerId player, ManaCost cost)
    {
        var plan = ManaPayment.Plan(State, player, cost);
        if (plan is null)
            return false;

        foreach (var (source, color) in plan)
        {
            Emit(new Tapped(source.Id));
            Emit(new ManaAdded(player, color, source.Amount));
        }

        var remaining = State.Player(player).Pool.Pay(cost);
        if (remaining is null)
            return false;
        Emit(new ManaPaid(player, remaining));
        return true;
    }

    void ResolveTop()
    {
        var item = State.Stack[^1];
        Emit(new ResolutionBegan(item.Id));

        if (item.Targets.Count > 0 && item.Targets.All(x => !TargetStillLegal(x)))
        {
            Emit(new Fizzled(item.Id));
            if (item.Spell is { } fizzledSpell)
                MoveToZone(fizzledSpell, Zone.Graveyard);
            return;
        }

        if (item.Spell is { } spellId && State.Object(spellId) is { } spell && spell.Card.IsPermanentCard)
        {
            var host = spell.Card.IsAura ? item.Targets.FirstOrDefault(x => x.IsObject).Object : null;
            MoveToZone(spellId, Zone.Battlefield, item.Controller, attachTo: host);
            Emit(new Resolved(item.Id));
            return;
        }

        var ctx = EffectContext.For(this, item, item.Subject);
        foreach (var effect in item.Effects)
            ctx = Interpreter.Execute(effect, ctx);
        if (item.Kind == StackKind.Triggered && item.DelayedId is { } delayedId)
            Emit(new DelayedTriggerRemoved(delayedId));

        if (item.Spell is { } cardId && State.Object(cardId)?.Zone == Zone.Stack)
            MoveToZone(cardId, Zone.Graveyard);

        Emit(new Resolved(item.Id));
    }

    bool TargetStillLegal(TargetRef target)
    {
        if (target.IsPlayer)
            return !State.Player(target.Player.Value).Lost;
        var obj = State.Object(target.Object.Value);
        return obj is not null && obj.Zone is Zone.Battlefield or Zone.Stack or Zone.Graveyard or Zone.Exile or Zone.Hand or Zone.Library;
    }

    public void CounterSpell(ObjectId spell)
    {
        var item = State.Stack.FirstOrDefault(x => x.Spell == spell);
        if (item is null)
            return;
        if (State.Object(spell) is { } obj && (CantBeCountered(obj) || Characteristics.Of(State, obj).Cant("BeCountered")))
        {
            Emit(new Note($"{obj.Card.Name} can't be countered"));
            return;
        }
        Emit(new Countered(item.Id));
        MoveToZone(spell, State.Object(spell) is { } countered ? Redirect(countered, Zone.Graveyard, countered: true) : Zone.Graveyard);
    }

    // ---- State-based actions and triggers -----------------------------------------------------------------------

    public void CheckStateAndTriggers()
    {
        for (var round = 0; round < 50; round++)
        {
            StateBasedActions();
            if (State.Over)
                return;
            if (!PutTriggersOnStack())
                return;
        }
    }

    void StateBasedActions()
    {
        for (var round = 0; round < 50; round++)
        {
            var changed = false;

            foreach (var player in State.Players.Where(x => !x.Lost))
                if (player.Life <= 0 || player.DrewFromEmptyLibrary || player.Poison >= 10)
                {
                    Emit(new PlayerLost(player.Id, player.Life <= 0 ? "life" : player.Poison >= 10 ? "poison" : "empty library"));
                    changed = true;
                }

            var inGame = State.PlayersInGame.ToList();
            if (inGame.Count <= 1)
            {
                Emit(new GameEnded(inGame.Count == 1 ? inGame[0] : null, "last player standing"));
                return;
            }

            foreach (var obj in State.Battlefield.ToList())
            {
                if (State.Object(obj.Id)?.Zone != Zone.Battlefield)
                    continue;
                var traits = Characteristics.Of(State, obj);
                if (traits.IsCreature)
                {
                    if (traits.Toughness <= 0)
                    {
                        MoveToZone(obj.Id, Zone.Graveyard);
                        changed = true;
                        continue;
                    }
                    if ((obj.Damage >= traits.Toughness || obj.CounterMap.ContainsKey("deathtouch-damage")) && Destroy(obj.Id))
                    {
                        changed = true;
                        continue;
                    }
                }
                if (obj.Card.IsSaga && obj.Card.FinalChapter is { } final && obj.CountersOf("lore") >= final && !State.Stack.Any(x => x.Source == obj.Id && x.Kind == StackKind.Triggered))
                {
                    Sacrifice(obj.Controller, obj.Id);
                    changed = true;
                    continue;
                }
                if (obj.Card.IsPlaneswalker && obj.CountersOf("loyalty") <= 0)
                {
                    MoveToZone(obj.Id, Zone.Graveyard);
                    changed = true;
                    continue;
                }
                if (obj.Card.IsAura && (obj.AttachedTo is null || State.Object(obj.AttachedTo.Value)?.Zone != Zone.Battlefield))
                {
                    MoveToZone(obj.Id, Zone.Graveyard);
                    changed = true;
                }
                if (obj.Card.IsEquipment && obj.AttachedTo is { } host && State.Object(host)?.Zone != Zone.Battlefield)
                {
                    Emit(new Attached(obj.Id, null));
                    changed = true;
                }
            }

            foreach (var token in State.Objects.Values.Where(x => x.IsToken && x.Zone != Zone.Battlefield && x.Zone != Zone.Stack).ToList())
            {
                Emit(new ObjectCeased(token.Id));
                changed = true;
            }

            if (!changed)
                return;
        }
    }

    /// <summary>Puts the abilities triggered by the events since the last check on the stack, active player's first; true when any were.</summary>
    bool PutTriggersOnStack()
    {
        var pending = _pending.ToList();
        _pending.Clear();
        if (pending.Count == 0)
            return false;

        List<PendingTrigger> triggered = [];
        foreach (var (evt, before, after) in pending)
            triggered.AddRange(Triggers.For(evt, after, before));
        // "Whenever one or more creatures die" triggers once for all that happened together.
        var once = new HashSet<(ObjectId, TriggeredAbility)>();
        triggered.RemoveAll(x => x.Ability.Trigger is ObjectEvent { Subject: ObjectSelector { Count: Special { Kind: "OneOrMore" } } } && !once.Add((x.Source.Id, x.Ability)));

        if (triggered.Count == 0)
            return false;

        var any = false;
        foreach (var controller in State.TurnOrderFrom(State.ActivePlayer))
            foreach (var trigger in triggered.Where(x => x.Controller == controller))
            {
                if (trigger.Ability.If is { } condition)
                {
                    var check = new EffectContext(this, controller, trigger.Source.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), TriggerSubject: trigger.Subject, Last: trigger.Subject is { } s ? [s] : null);
                    if (Conditions.Evaluate(condition, check) != true)
                        continue;
                }

                var slots = TargetSlots.Of([trigger.Ability.Effect is ModalEffect modal ? new Sequence(modal.Modes.Select(m => m.Effect).ToList()) : trigger.Ability.Effect]).ToList();
                var targets = ChooseTargetsFor(slots, controller, trigger.Source.Id);
                if (targets is null)
                    continue;

                var (effect, bindings) = ResolveModes(trigger.Ability.Effect, controller);
                var item = new StackItem(State.NextStackId, controller, trigger.Source.Zone == Zone.Command ? null : trigger.Source.Id, StackKind.Triggered, [effect], targets, 0,
                    Description: $"{trigger.Source.Card.Name} trigger", Subject: trigger.Subject, Bindings: bindings, DelayedId: trigger.Delayed?.Id);
                if (trigger.Delayed is { } fired)
                    Emit(new DelayedTriggerRemoved(fired.Id));
                Emit(new Triggered(item));
                any = true;
            }

        // Putting triggers on the stack emits events of its own; those don't trigger anything, so drop them.
        _pending.Clear();
        return any;
    }

    // ---- Primitive actions ----------------------------------------------------------------------------------------

    public void MoveToZone(ObjectId id, Zone zone, PlayerId? controller = null, bool tapped = false, int position = -1, ObjectId? attachTo = null)
    {
        var obj = State.Object(id);
        if (obj is null)
            return;
        zone = Redirect(obj, zone, countered: false);
        var to = controller ?? (zone == Zone.Battlefield ? obj.Controller : obj.Owner);

        List<Effect> onEntry = [];
        if (zone == Zone.Battlefield)
            tapped |= EntryReplacements(obj, to, onEntry);

        Emit(new ZoneMoved(id, obj.Zone, zone, to, tapped, position, attachTo is not null, attachTo, obj.Zone == Zone.Battlefield ? obj.Controller : null));

        // What this permanent exiled "until it leaves the battlefield" comes back.
        if (obj.Zone == Zone.Battlefield && zone != Zone.Battlefield)
            foreach (var link in State.LinkedExileList.Where(x => x.Source == id && x.SourceZoneChanges == obj.ZoneChangeCount).ToList())
            {
                Emit(new LinkedExileEnded(id));
                foreach (var (card, changes) in link.Exiled)
                    if (State.Object(card) is { Zone: Zone.Exile } exiled && exiled.ZoneChangeCount == changes)
                        MoveToZone(card, Zone.Battlefield, exiled.Owner);
            }

        if (zone == Zone.Battlefield)
        {
            if (obj.Card.IsPlaneswalker && obj.Card.Loyalty is { } loyalty)
                Emit(new CountersChanged(id, "loyalty", loyalty));
            if (obj.Card.IsSaga)
                Emit(new CountersChanged(id, "lore", 1));
            if (onEntry.Count > 0)
            {
                var ctx = new EffectContext(this, to, id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), Last: [TargetRef.Of(id)]);
                foreach (var effect in onEntry)
                    Interpreter.Execute(effect, ctx);
            }
        }
    }

    /// <summary>
    /// How <paramref name="obj"/> enters the battlefield under its own and others' replacement abilities: whether it
    /// enters tapped, and the effects that happen as it enters (counters, choices). "Enters tapped unless …" applies
    /// when its condition can't be judged.
    /// </summary>
    bool EntryReplacements(GameObject obj, PlayerId controller, List<Effect> onEntry)
    {
        var tapped = false;
        var entering = obj with { Zone = Zone.Battlefield, Controller = controller };

        void Consider(ReplacementAbility replacement, GameObject source)
        {
            switch (replacement.Effect)
            {
                case EntersBattlefield enters:
                {
                    var self = enters.Objects is null or Self;
                    if (self ? source.Id != obj.Id : !Applies(enters.Objects, source, entering))
                        return;
                    if (enters.If is { } condition && Conditions.Evaluate(condition, EffectContext.Snapshot(State, controller, obj.Id)) == false)
                        return;
                    if (enters.States?.Contains("Tapped") == true)
                        tapped = true;
                    onEntry.AddRange(enters.With ?? []);
                    return;
                }
                case AsEventHappens { When: ObjectEvent { Subject: Self or null } when } happens when when.Events.Any(x => x is "EntersTheBattlefield" or "EntersOrTransforms" or "Enters") && source.Id == obj.Id:
                    onEntry.Add(happens.Effect);
                    return;
            }
        }

        foreach (var replacement in obj.Card.Abilities.OfType<ReplacementAbility>())
            Consider(replacement, entering);
        // "If this creature was kicked, it enters with …": an entry replacement stated as an effect line under a condition.
        foreach (var line in obj.Card.Abilities.OfType<EffectLine>())
            if (line.Effect is Conditional { Then: EntersBattlefield { Objects: null or Self } enters, Else: null } conditional
                && Conditions.Evaluate(conditional.If, EffectContext.Snapshot(State, controller, obj.Id)) == true)
                Consider(new ReplacementAbility(enters), entering);
        foreach (var other in State.Battlefield.Where(x => x.Id != obj.Id))
            foreach (var replacement in other.Card.Abilities.OfType<ReplacementAbility>())
                Consider(replacement, other);

        return tapped;
    }

    bool Applies(Selector objects, GameObject source, GameObject obj) =>
        objects switch
        {
            ObjectSelector selector => Filters.Matches(State, selector.Filter, obj, source.Controller, source.Id, selector.Other ? source.Id : null),
            Union union => union.Members.Any(x => Applies(x, source, obj)),
            _ => false,
        };

    public void Draw(PlayerId player, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var library = State.Player(player).Library;
            Emit(new Drew(player, library.Count > 0 ? library[0] : null));
        }
    }

    public void Shuffle(PlayerId player)
    {
        var order = State.Player(player).Library.OrderBy(_ => Rng.Next()).ToImmutableList();
        Emit(new LibraryShuffled(player, order));
    }

    public void ChangeLife(PlayerId player, int delta, string reason)
    {
        if (delta == 0 || State.Player(player).Lost)
            return;
        Emit(new LifeChanged(player, delta, reason));
    }

    public void DealDamage(ObjectId? source, TargetRef target, int amount, bool combat)
    {
        if (amount <= 0)
            return;
        amount = Prevent(source, target, amount, combat);
        if (amount <= 0)
            return;
        var dealer = source is { } s ? State.Object(s) : null;
        var traits = dealer is { Zone: Zone.Battlefield } ? Characteristics.Of(State, dealer) : null;
        var lifelink = traits?.HasKeyword("Lifelink") == true;
        var deathtouch = traits?.HasKeyword("Deathtouch") == true;
        var infect = traits?.HasKeyword("Infect") == true;

        if (target.IsPlayer)
        {
            if (State.Player(target.Player.Value).Lost)
                return;
            Emit(new DamageDealt(source, target, amount, combat, lifelink, deathtouch));
            if (infect)
                Emit(new PoisonChanged(target.Player.Value, amount));
            else
                Emit(new LifeChanged(target.Player.Value, -amount, "damage"));
        }
        else
        {
            var victim = State.Object(target.Object.Value);
            if (victim is null || victim.Zone != Zone.Battlefield)
                return;
            Emit(new DamageDealt(source, target, amount, combat, lifelink, deathtouch));
            if (victim.Card.IsPlaneswalker)
                Emit(new CountersChanged(victim.Id, "loyalty", -amount));
            if (infect)
                Emit(new CountersChanged(victim.Id, "-1/-1", amount));
        }

        if (lifelink && dealer is not null)
            ChangeLife(dealer.Controller, amount, "lifelink");
    }

    public bool Destroy(ObjectId id, bool cantBeRegenerated = false)
    {
        var obj = State.Object(id);
        if (obj is null || obj.Zone != Zone.Battlefield)
            return false;
        var traits = Characteristics.Of(State, obj);
        if (traits.HasKeyword("Indestructible"))
            return false;
        if (!cantBeRegenerated && !traits.Cant("BeRegenerated") && obj.CountersOf("regeneration-shield") > 0)
        {
            Emit(new CountersChanged(id, "regeneration-shield", -1));
            Emit(new Tapped(id));
            Emit(new RemovedFromCombat(id));
            Emit(new DamageRemovedFrom(id));
            return true;
        }
        Emit(new Destroyed(id));
        MoveToZone(id, Zone.Graveyard);
        return true;
    }

    public bool Sacrifice(PlayerId player, ObjectId id)
    {
        var obj = State.Object(id);
        if (obj is null || obj.Zone != Zone.Battlefield || obj.Controller != player)
            return false;
        Emit(new Sacrificed(player, id));
        MoveToZone(id, Zone.Graveyard);
        return true;
    }

    public void Discard(PlayerId player, ObjectId id)
    {
        if (State.Object(id)?.Zone != Zone.Hand)
            return;
        Emit(new Discarded(player, id));
        MoveToZone(id, Zone.Graveyard);
    }

    public void Tap(ObjectId id)
    {
        if (State.Object(id) is { Zone: Zone.Battlefield, Tapped: false })
            Emit(new Tapped(id));
    }

    public void Untap(ObjectId id)
    {
        if (State.Object(id) is { Zone: Zone.Battlefield, Tapped: true })
            Emit(new Untapped(id));
    }

    /// <summary>A token copy of a card, for populate and the like.</summary>
    public ObjectId CreateCopyToken(CardDefinition original, PlayerId controller)
    {
        var id = new ObjectId(State.NextObjectId);
        Emit(new ObjectCreated(new GameObject(id, original with { IsToken = true }, controller, controller, Zone.Battlefield, EnteredThisTurn: true)));
        Emit(new ZoneMoved(id, Zone.Command, Zone.Battlefield, controller));
        return id;
    }

    /// <summary>Pays a mana cost from the pool and untapped sources; false (and nothing paid) when it can't be.</summary>
    public bool PayMana(PlayerId player, ManaCost cost) => Pay(player, cost);

    public ObjectId CreateToken(TokenSpec spec, PlayerId controller)
    {
        var id = new ObjectId(State.NextObjectId);
        var definition = CardDefinition.FromToken(spec);
        Emit(new ObjectCreated(new GameObject(id, definition, controller, controller, Zone.Battlefield, Tapped: spec.Tapped, EnteredThisTurn: true, Attacking: spec.Attacking)));
        Emit(new ZoneMoved(id, Zone.Command, Zone.Battlefield, controller, spec.Tapped));
        return id;
    }

    public void AddManaSymbol(PlayerId player, string symbol)
    {
        var inner = symbol.Trim('{', '}');
        if (Colors.OfSymbol(inner) is { } color)
            Emit(new ManaAdded(player, color, 1));
        else if (inner.Equals("C", StringComparison.OrdinalIgnoreCase))
            Emit(new ManaAdded(player, null, 1));
        else if (int.TryParse(inner, out var amount))
            Emit(new ManaAdded(player, null, amount));
        else if (inner.Contains('/') && inner.Split('/').Select(Colors.OfSymbol).FirstOrDefault(x => x is not null) is { } first)
            Emit(new ManaAdded(player, first, 1));
    }

    public void Win(PlayerId player)
    {
        foreach (var other in State.PlayersInGame.Where(x => x != player).ToList())
            Emit(new PlayerLost(other, "opponent won"));
    }
}
