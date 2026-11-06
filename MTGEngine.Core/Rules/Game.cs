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

    /// <summary>The extra mana the modes just chosen cost ("+ {1} — …"), added to the spell being cast.</summary>
    ManaCost _modeCost = ManaCost.Free;
    bool _skippingCombat;
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

    /// <summary>Whether the last coin flip was won, for "if you win the flip".</summary>
    public bool WonLastFlip { get; set; }

    /// <summary>The last die roll's result, for a die's table of results.</summary>
    public int LastDieRoll { get; set; }
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

    /// <summary>More events or decisions than any real turn has: the game is looping, and stops with what it was doing rather than hang.</summary>
    const int RunawayEvents = 50_000, RunawayDecisions = 10_000, RunawayObjects = 1_500;
    int _eventsThisTurn, _decisionsThisTurn;

    public void Emit(GameEvent evt)
    {
        var before = State;
        State = Reducer.Apply(State, evt);
        _log.Add(evt);
        _pending.Add((evt, before, State));
        _eventsThisTurn = evt is TurnBegan ? 0 : _eventsThisTurn + 1;
        if (evt is TurnBegan)
            _decisionsThisTurn = 0;
        if (_eventsThisTurn > RunawayEvents)
            throw Runaway("events");
        if (evt is LifeChanged { Delta: < 0 } lost)
            RaiseSpeed(lost.Player);
        if (evt is TemporaryEffectAdded && State.Temporary.Count > RunawayObjects)
            throw Runaway("temporary effects: " + string.Join(", ", State.Temporary.GroupBy(x => State.Object(x.Target)?.Card.Name + (x.UntilEndOfTurn is null ? " (no end)" : "")).OrderByDescending(x => x.Count()).Take(5).Select(x => $"{x.Count()} on {x.Key}")) + "; last " + evt);
        if (evt is ObjectCreated && State.Objects.Count > RunawayObjects)
            throw Runaway("objects: " + string.Join(", ", State.Objects.Values.GroupBy(x => x.Card.Name).OrderByDescending(x => x.Count()).Take(5).Select(x => $"{x.Count()} {x.Key}")));
    }

    public DecisionResponse Decide(Decision decision)
    {
        if (++_decisionsThisTurn > RunawayDecisions)
            throw Runaway("decisions: last " + decision);
        var response = _agents[decision.Player.Value].Decide(State, decision) ?? new DecisionResponse();
        return response;
    }

    InvalidOperationException Runaway(string what)
    {
        var turn = _log.Skip(_log.FindLastIndex(x => x is TurnBegan) is var start and >= 0 ? start : 0).ToList();
        var activated = turn.OfType<AbilityActivated>().GroupBy(x => x.Item.Description).OrderByDescending(x => x.Count()).Take(3).Select(x => $"{x.Count()}x {x.Key}");
        var mana = turn.OfType<ManaAdded>().Count();
        return new($"runaway turn ({what}); most activated this turn: {string.Join("; ", activated)}; mana added {mana} times; last events: " + string.Join(" | ", _log.TakeLast(8)));
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

    /// <summary>The step "skip your next … step" names, for the steps the engine can skip (a whole combat is its beginning step).</summary>
    public static Step? SkippableStep(string phase) =>
        phase switch
        {
            "UntapStep" or "Untap" => Step.Untap,
            "UpkeepStep" or "Upkeep" => Step.Upkeep,
            "DrawStep" or "Draw" => Step.Draw,
            "CombatPhase" or "Combat" => Step.BeginCombat,
            _ => null,
        };

    void BeginStep(Step step)
    {
        var active = State.ActivePlayer;

        // "You skip your next untap step"; skipping combat skips all its steps.
        if (State.SkipList.Contains((active, step)))
        {
            Emit(new StepSkipped(active, step));
            if (step == Step.BeginCombat)
                _skippingCombat = true;
            return;
        }
        if (_skippingCombat && step is Step.DeclareAttackers or Step.DeclareBlockers or Step.CombatDamage or Step.EndCombat)
            return;
        _skippingCombat = false;

        if (step is Step.DeclareBlockers or Step.CombatDamage && !_attackersThisCombat)
            return;

        Emit(new StepBegan(step, active));

        switch (step)
        {
            case Step.Untap:
                Emit(new SummoningSicknessCleared(active));
                foreach (var obj in State.Battlefield.Where(x => x.Controller == active && x.Tapped).ToList())
                    if (!DoesntUntap(obj) && !(MayStayTapped(obj) && !Decide(new YesNo(active, $"Untap {obj.Card.Name}?")).Yes))
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

            case Step.Upkeep:
                TickSuspended(active);
                Priority();
                return;

            default:
                Priority();
                return;
        }
    }

    /// <summary>How many lands <paramref name="player"/> may play this turn: one, plus "you may play an additional land on each of your turns" and "… this turn".</summary>
    int LandPlays(PlayerId player)
    {
        var plays = 1 + State.Player(player).ExtraLandsThisTurn;
        foreach (var (effect, _, source) in Characteristics.StaticEffects(State))
            if (effect is ExtraLandPlays { EachTurn: true } extra && Resolver.Players(State, extra.Player ?? new PlayerSelector(PlayerKind.You), source.Controller, source.Id).Contains(player))
                plays += extra.Count is Special { Kind: "AnyNumber" } ? 1000 : (extra.Count as Fixed)?.Value ?? 1;
        return plays;
    }

    /// <summary>"You may choose not to untap this artifact during your untap step."</summary>
    bool MayStayTapped(GameObject obj) =>
        Characteristics.AbilitiesOf(State, obj).Any(x => x is StaticAbility { Effect: MayNotUntap { Objects: Self or null } });

    bool DoesntUntap(GameObject obj) =>
        State.Temporary.Any(x => x.Target == obj.Id && x.DoesntUntap && x.ActiveIn(State))
        || Characteristics.StaticEffects(State).Any(x => x.Effect is DoesntUntap && Characteristics.Covers(State, x.Objects, x.Source, obj));

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
        foreach (var player in State.Players.Where(x => !x.Pool.IsEmpty || x.RestrictedList.Count > 0))
            Emit(new PoolEmptied(player.Id));
    }

    // ---- Priority and the stack -------------------------------------------------------------------------------

    void Priority()
    {
        var player = State.ActivePlayer;
        var passes = 0;
        var stalls = 0;
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
            var before = State;
            Perform(action, player, response);
            // An action that changed nothing (a cast that couldn't be paid after all) three times running is taken as a pass.
            // Events that change nothing leave the very same state, so the reference says whether anything happened.
            stalls = ReferenceEquals(State, before) ? stalls + 1 : 0;
            if (stalls >= 3)
            {
                stalls = 0;
                Emit(new Passed(player));
                passes++;
                player = State.NextInTurnOrder(player);
            }
        }
    }

    /// <summary>The restrictions on what <paramref name="player"/> may cast or play now: those set up for a while, and static ones on permanents.</summary>
    IEnumerable<PlayRestriction> RestrictionsOn(PlayerId player)
    {
        foreach (var restriction in State.PlayRestrictionList.Where(x => x.Player == player))
            yield return restriction;
        foreach (var source in State.Battlefield.ToList())
            foreach (var ability in Characteristics.AbilitiesOf(State, source).OfType<StaticAbility>())
            {
                if (ability.Effect is CastAsThoughFlash { Extra: null } asFlash && source.Controller == player)
                {
                    yield return new PlayRestriction(player, "Flash", asFlash.Spells, source.Id, source.Controller, null);
                    continue;
                }
                if (ability.Effect is CantPlay cantPlay)
                {
                    var ctx = new EffectContext(this, source.Controller, source.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
                    if (ability.While is { } condition && Conditions.Evaluate(condition, ctx) != true)
                        continue;
                    if (Resolver.Players(cantPlay.Player, ctx).Contains(player))
                        yield return new PlayRestriction(player, cantPlay.Verb, cantPlay.What, source.Id, source.Controller, null, cantPlay.OrActivate is not null, cantPlay.MoreThan);
                }
            }
    }

    /// <summary>Whether <paramref name="player"/> may cast <paramref name="card"/> as though it had flash ("you may cast creature spells as though they had flash").</summary>
    bool AsThoughFlash(PlayerId player, GameObject card) =>
        RestrictionsOn(player).Any(x => x.Verb == "Flash"
            && (x.What is not ObjectSelector { Filter: var filter } || Filters.Matches(State, filter with { Zone = null }, card with { Zone = Zone.Stack, Controller = player }, x.Controller, x.Source)));

    /// <summary>Whether a restriction forbids <paramref name="player"/> casting (or playing, for a land) <paramref name="card"/> now.</summary>
    bool Forbidden(PlayerId player, GameObject card)
    {
        foreach (var restriction in RestrictionsOn(player))
        {
            if (restriction.Verb is not ("Cast" or "Play") || restriction.Verb == "Cast" && card.Card.IsLand)
                continue;
            if (restriction.MoreThan is { } most)
            {
                var ctx = new EffectContext(this, restriction.Controller, restriction.Source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
                if (State.ThisTurnEventList.OfType<SpellCast>().Count(x => x.Item.Controller == player) < (Quantities.Evaluate(most, ctx) ?? 1))
                    continue;
            }
            var asCast = card with { Zone = card.Card.IsLand ? Zone.Battlefield : Zone.Stack, Controller = player };
            if (restriction.What is not ObjectSelector { Filter: var filter } || Filters.Matches(State, filter with { Zone = null }, asCast, restriction.Controller, restriction.Source))
                return true;
        }
        return false;
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

            if (Forbidden(player, obj))
                continue;
            if (card.IsLand)
            {
                if (sorcerySpeed && playerState.LandsPlayedThisTurn < LandPlays(player))
                    actions.Add(new PlayLand(id));
                continue;
            }

            var instantSpeed = card.IsInstant || card.Keywords.Contains("Flash") || AsThoughFlash(player, obj);
            if (!instantSpeed && !sorcerySpeed)
                continue;
            if (!card.Abilities.OfType<ActivationLimits>().Where(x => x.ForCasting).SelectMany(x => x.Timings).All(t => TimingAllows(t, obj, player)))
                continue;
            if (!TargetsAvailable(SpellSlots(card), player, id) || !ModesAvailable(card, player, id))
                continue;
            if (CanAffordToCast(obj, player))
                actions.Add(new CastSpell(id, card.ManaCost.X > 0 ? PayableX(obj, player, ManaPayment.MaxX(state, player, CostToCast(obj, player, card.ManaCost, null))) : 0));
            // Morph and disguise: cast it face down as a 2/2 creature for {3}, at sorcery speed.
            if (sorcerySpeed && FaceUpKeyword(card) is not null && ManaPayment.CanPay(state, player, ManaCost.Parse("{3}")))
                actions.Add(new CastSpell(id, 0, Alternative: FaceDownCast));
            foreach (var (index, alternative) in Alternatives(card))
                if (CanAffordAlternative(obj, player, alternative))
                    actions.Add(new CastSpell(id, 0, Alternative: index));
            actions.AddRange(KeywordCastActions(obj, player));
            if (CanSuspend(obj, player, sorcerySpeed))
                actions.Add(new CastSpell(id, 0, Alternative: SuspendCast));
        }

        actions.AddRange(FaceUpActions(player));
        actions.AddRange(ForetellActions(player, sorcerySpeed));
        actions.AddRange(PlotActions(player, sorcerySpeed));
        actions.AddRange(SneakActions(player));

        // Abilities that work from the hand or graveyard (cycling, unearth), and flashback.
        foreach (var obj in playerState.Hand.Concat(playerState.Graveyard).Select(state.Object).Where(x => x is not null).ToList())
        {
            var abilities = obj.Card.Abilities;
            for (var index = 0; index < abilities.Count; index++)
            {
                if (abilities[index] is not ZoneAbility zoned || zoned.Zone != obj.Zone)
                    continue;
                var limits = index + 1 < abilities.Count && abilities[index + 1] is ActivationLimits following ? following.Timings : [];
                if (limits.Any(t => t.Kind == nameof(TimingKind.SorcerySpeed)) && !sorcerySpeed)
                    continue;
                // Ninjutsu: once blockers are declared, on your own turn.
                if (zoned.Ability.Effect is NinjutsuEnter && (state.Step != Step.DeclareBlockers || state.ActivePlayer != player))
                    continue;
                if (!CanPayCosts(obj, zoned.Ability.Costs, player) || !TargetsAvailable(TargetSlots.Of([zoned.Ability.Effect]).ToList(), player, obj.Id))
                    continue;
                actions.Add(new ActivateAbility(obj.Id, index, Interpreter.Describe(zoned.Ability.Effect)));
            }
            if (obj.Zone == Zone.Graveyard && FlashbackCost(obj.Card) is { } flashback && !Forbidden(player, obj)
                && (obj.Card.IsInstant || obj.Card.Keywords.Contains("Flash") || sorcerySpeed)
                && AdditionalPayable(flashback, player, obj.Id, 0) && AdditionalCostsOf(obj.Card).All(cost => AdditionalPayable(cost, player, obj.Id, 0))
                && TargetsAvailable(SpellSlots(obj.Card), player, obj.Id))
                actions.Add(new CastSpell(obj.Id, 0, Alternative: Flashback));
            actions.AddRange(GraveyardCastActions(obj, player, sorcerySpeed));
        }

        foreach (var (permission, obj) in Permitted(player).Concat(StaticPermitted(player)).DistinctBy(x => x.Card.Id).ToList())
        {
            var card = obj.Card;
            if (card.IsLand)
            {
                if (sorcerySpeed && playerState.LandsPlayedThisTurn < LandPlays(player))
                    actions.Add(new PlayLand(obj.Id));
                continue;
            }
            if (!(card.IsInstant || card.Keywords.Contains("Flash")) && !sorcerySpeed)
                continue;
            if (!permission.Free && !CanAffordToCast(obj, player))
                continue;
            if (!TargetsAvailable(SpellSlots(card), player, obj.Id))
                continue;
            actions.Add(new CastSpell(obj.Id, !permission.Free && card.ManaCost.X > 0 ? PayableX(obj, player, ManaPayment.MaxX(state, player, CostToCast(obj, player, card.ManaCost, null))) : 0, permission.Free));
        }

        var noActivating = RestrictionsOn(player).Any(x => x.OrActivate);
        // "Any player may activate this ability" lets others activate it too.
        foreach (var obj in state.Battlefield.Where(x => !noActivating && (x.Controller == player || x.Card.Abilities.OfType<ActivationLimits>().Any(l => l.Timings.Any(t => t.Kind == "AnyPlayer")))))
        {
            var traits = Characteristics.Of(state, obj);
            if (traits.LostAbilities || traits.Cant("Activate") || traits.Cant("ActivateNonMana"))
                continue;
            var abilities = traits.Abilities;
            var index = -1;
            foreach (var ability in abilities)
            {
                index++;
                // "Activate only as a sorcery" follows the ability it limits.
                var limits = index + 1 < abilities.Count && abilities[index + 1] is ActivationLimits following ? following.Timings.ToList() : [];
                var sorceryOnly = limits.Any(t => t.Kind == nameof(TimingKind.SorcerySpeed));
                var onceEachTurn = limits.Any(t => t.Kind is nameof(TimingKind.OnceEachTurn) or nameof(TimingKind.Once));
                switch (ability)
                {
                    case ActivatedAbility activated when obj.Controller == player
                        || limits.Any(t => t.Kind == "AnyPlayer") && !Flatten(activated.Costs).Any(c => c is TapSelf or UntapSelf or ActionCost { Action: MTGRulesIR.Model.Sacrifice { Objects: Self } }):
                        // Mana abilities that only tap are used as payment needs them; others ("{1}: add {r}", "sacrifice this: add {c}") are activated for the pool.
                        if (activated.Effect is AddMana && activated.Costs.All(x => x is TapSelf))
                            continue;
                        if (sorceryOnly && !sorcerySpeed)
                            continue;
                        // "Max speed — {ability}" can be activated only at max speed.
                        if (activated.Effect is Conditional { If: AtMaxSpeed, Else: null } && state.Player(player).Speed < 4)
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
                    case LoyaltyAbility loyalty when obj.Controller == player:
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

        if (SplitSecond())
            actions.RemoveAll(x => x is CastSpell { Alternative: not (ForetellExile or PlotExile or SuspendCast) } || x is ActivateAbility activate && !IsManaAbility(activate));

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
    (Effect Effect, ImmutableDictionary<string, ImmutableList<TargetRef>> Bindings) ResolveModes(Effect effect, PlayerId player, ManaCost entwine = null, ObjectId? source = null)
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
        var entwined = entwine is not null && Decide(new YesNo(player, $"Pay entwine {entwine}?")).Yes;
        if (entwined)
            chosen.AddRange(Enumerable.Range(0, modal.Modes.Count));
        for (var i = 0; i < max && !entwined; i++)
        {
            var remaining = Enumerable.Range(0, modal.Modes.Count).Where(m => !chosen.Contains(m)).ToList();
            // A mode can be chosen only if its targets can be ("counter target spell" with nothing on the stack can't).
            var possible = remaining.Where(m => TargetsAvailable(TargetSlots.Of([modal.Modes[m].Effect]).ToList(), player, source)).ToList();
            if (possible.Count > 0 || chosen.Count >= min)
                remaining = possible;
            if (remaining.Count == 0)
                break;
            if (chosen.Count >= min && i > 0 && !Decide(new YesNo(player, "Choose another mode?")).Yes)
                break;
            var pick = Decide(new ChooseOne(player, "Choose a mode", remaining.Select(m => options[m]).ToImmutableList())).Index;
            chosen.Add(remaining[Math.Clamp(pick, 0, remaining.Count - 1)]);
        }

        Emit(new ModesChosen(player, chosen.ToImmutableList()));
        _modeCost = chosen.Select(m => modal.Modes[m].Cost).OfType<PayMana>().Aggregate(entwined ? entwine : ManaCost.Free, (sum, cost) => sum.Plus(ManaCost.Parse(string.Concat(cost.Symbols))));
        var steps = chosen.Select(m => modal.Modes[m].Effect).ToList();
        return (steps.Count == 1 ? steps[0] : new Sequence(steps), null);
    }

    /// <summary>Whether a modal spell has enough modes whose targets can be chosen ("choose one —" with each mode's target missing can't be cast).</summary>
    bool ModesAvailable(CardDefinition card, PlayerId player, ObjectId source) =>
        card.SpellEffects.OfType<ModalEffect>().All(modal =>
            modal.Modes.Count(m => TargetsAvailable(TargetSlots.Of([m.Effect]).ToList(), player, source)) >= (modal.Count is "AnyNumber" or "UpToOne" or "UpToTwo" or "UpToFour" or "UpToX" or "UpToThatMany" ? 0 : modal.Count is "Two" or "Both" ? 2 : modal.Count == "Three" ? 3 : modal.Count == "Four" ? 4 : 1));

    /// <summary>The largest X up to <paramref name="max"/> the spell's additional costs allow too ("as an additional cost, discard x cards").</summary>
    int PayableX(GameObject obj, PlayerId player, int max)
    {
        var costs = AdditionalCostsOf(obj.Card).ToList();
        var x = max;
        while (x > 0 && !costs.All(cost => AdditionalPayable(cost, player, obj.Id, x)))
            x--;
        return x;
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
                    if (State.Player(player).Energy < EnergyIn(mana) || !ManaPayment.CanPay(State, player, ActivationCost(source, mana), purpose: new ManaPayment.Purpose(Activating: source)))
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
                case DiscardThis:
                    if (source.Zone != Zone.Hand)
                        return false;
                    break;
                case TapCreaturesWithPower crew:
                    if (CrewCandidates(source, player).Sum(x => Characteristics.Of(State, x).Power) < crew.Power)
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

    /// <summary>An activated ability's mana cost after "this ability costs {1} less to activate for each …" on its permanent.</summary>
    ManaCost ActivationCost(GameObject source, PayMana mana)
    {
        var cost = ManaCost.Parse(string.Concat(mana.Symbols.Where(x => !IsEnergy(x))));
        var reduction = 0;
        foreach (var ability in source.Card.Abilities)
            foreach (var (modification, condition, variable) in CostModifications(ability))
            {
                if (modification.Spells is not SelfAbility || modification.Action != "Activate")
                    continue;
                var ctx = new EffectContext(this, source.Controller, source.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
                if (variable is { } let && Quantities.Evaluate(let.Definition, ctx) is { } value)
                    ctx = ctx with { X = value };
                if (condition is not null && Conditions.Evaluate(condition, ctx) != true || modification.Condition is not null && Conditions.Evaluate(modification.Condition, ctx) != true)
                    continue;
                var times = modification.PerEach is null ? 1 : Quantities.Evaluate(modification.PerEach, ctx) ?? 0;
                var amount = ManaCost.Parse(string.Concat(modification.Amount?.Symbols ?? [])).Generic * times;
                reduction += modification.Change == "Less" ? amount : -amount;
            }
        return cost.AdjustGeneric(-reduction);
    }

    /// <summary>The untapped creatures besides the vehicle that could crew it.</summary>
    IEnumerable<GameObject> CrewCandidates(GameObject vehicle, PlayerId player) =>
        State.Battlefield.Where(x => x.Controller == player && x.Id != vehicle.Id && !x.Tapped && Characteristics.Of(State, x).IsCreature);

    /// <summary>The alternative index that marks casting with flashback.</summary>
    const int Flashback = -2;

    /// <summary>The alternative index that marks casting for a madness cost.</summary>
    const int MadnessCast = -4;

    /// <summary>A card's flashback cost, if it has flashback.</summary>
    static Cost FlashbackCost(CardDefinition card) => KeywordCost(card, "Flashback");

    /// <summary>The cost a keyword of <paramref name="card"/> names ("madness {1}{r}"), if it has that keyword.</summary>
    static Cost KeywordCost(CardDefinition card, string keyword) =>
        card.Abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords).FirstOrDefault(x => CardDefinition.Normalize(x.Keyword) == keyword)?.Cost;

    static bool IsEnergy(string symbol) => symbol.Equals("{e}", StringComparison.OrdinalIgnoreCase);

    /// <summary>How many energy counters a cost's symbols ask for ("pay {e}{e}").</summary>
    static int EnergyIn(PayMana mana) => mana.Symbols.Count(IsEnergy);

    static IEnumerable<Cost> Flatten(IEnumerable<Cost> costs) =>
        costs.SelectMany(x => x is CompoundCost compound ? Flatten(compound.Parts) : [KeywordAbilities.CostOf(x)]);

    IEnumerable<GameObject> SacrificeCandidates(Sacrifice sacrifice, PlayerId player, ObjectId source)
    {
        var ctx = new EffectContext(this, player, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
        if (sacrifice.Objects is ObjectSelector selector)
            return State.Battlefield.Where(x => x.Controller == player && Filters.Matches(State, selector.Filter, x, player, source, selector.Other ? source : null, ctx: ctx));
        return [];
    }

    /// <summary>Takes a legal action for <paramref name="player"/> outside the priority loop (tools and tests).</summary>
    public void Take(LegalAction action, PlayerId player) => Perform(action, player, new DecisionResponse());

    void Perform(LegalAction action, PlayerId player, DecisionResponse response)
    {
        switch (action)
        {
            case PlayLand land:
                MoveToZone(land.Card, Zone.Battlefield, player);
                Emit(new LandPlayed(player, land.Card));
                return;
            case CastSpell { Alternative: SuspendCast } suspend:
                Suspend(suspend.Card, player);
                return;
            case CastSpell { Alternative: PlotExile } plot:
                Plot(plot.Card, player);
                return;
            case CastSpell { Alternative: ForetellExile } foretell:
                Foretell(foretell.Card, player);
                return;
            case CastSpell cast:
                Cast(cast.Card, player, cast.MaxX, cast.Free, cast.Alternative);
                return;
            case TurnFaceUp turn:
                TurnUp(turn.Permanent, pay: true);
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
        if (found is ZoneAbility { Ability: var fromZone } zoneAbility && zoneAbility.Zone == obj.Zone)
            found = fromZone;

        if (found is LoyaltyAbility loyalty)
        {
            var change = (loyalty.Change as Fixed)?.Value ?? 0;
            var (loyaltyEffect, loyaltyBindings) = ResolveModes(loyalty.Effect, player, source: source);
            var loyaltySlots = TargetSlots.Of([loyaltyEffect]).ToList();
            var loyaltyTargets = ChooseTargetsFor(loyaltySlots, player, source);
            if (loyaltyTargets is null)
                return;
            var loyaltyCounts = _lastTargetCounts;
            if (change != 0)
                Emit(new CountersChanged(source, "loyalty", change));
            var loyaltyItem = new StackItem(State.NextStackId, player, source, StackKind.Activated, [loyaltyEffect], loyaltyTargets, 0, Description: $"{obj.Card.Name} [{change:+#;-#;0}]", AbilityIndex: index, Bindings: loyaltyBindings, TargetCounts: loyaltyCounts);
            Emit(new AbilityActivated(loyaltyItem));
            return;
        }

        if (found is not ActivatedAbility ability)
            return;

        var (effect, bindings) = ResolveModes(ability.Effect, player, source: source);
        var slots = TargetSlots.Of([effect]).ToList();
        var targets = ChooseTargetsFor(slots, player, source);
        if (targets is null)
            return;
        var targetCounts = _lastTargetCounts;

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
                    if (EnergyIn(mana) is > 0 and var energy)
                        Emit(new EnergyChanged(player, -energy));
                    if (!Pay(player, ActivationCost(obj, mana), purpose: new ManaPayment.Purpose(Activating: obj)))
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
                // "Tap an untapped creature you control": chosen among the untapped ones (an effect's "tap a creature" may pick any).
                case ActionCost { Action: Tap { Objects: ObjectSelector { Quantifier: not Quantifier.Target } tapping } }:
                {
                    var tapCtx = new EffectContext(this, player, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
                    var count = tapping.Quantifier == Quantifier.Count ? Quantities.Evaluate(tapping.Count, tapCtx) ?? 1 : 1;
                    var untapped = Resolver.Candidates(tapping.Filter, tapCtx)
                        .Where(o => o.Zone == Zone.Battlefield && !o.Tapped && Filters.Matches(State, tapping.Filter, o, player, source, tapping.Other ? source : null, ctx: tapCtx))
                        .Select(o => TargetRef.Of(o.Id)).ToImmutableList();
                    var chosen = untapped.Count <= count ? untapped : Decide(new ChooseTargets(player, "Tap", untapped, count, count)).Targets ?? untapped.Take(count).ToImmutableList();
                    foreach (var tapped in chosen.Where(x => x.IsObject).Take(count).ToList())
                    {
                        Emit(new Tapped(tapped.Object.Value));
                        paid.Add(tapped);
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
                case DiscardThis:
                    Discard(player, source);
                    paid.Add(TargetRef.Of(source));
                    break;
                case TapCreaturesWithPower crew:
                {
                    // Tap the strongest first until the total is enough.
                    var total = 0;
                    foreach (var creature in CrewCandidates(obj, player).OrderByDescending(x => Characteristics.Of(State, x).Power).ToList())
                    {
                        if (total >= crew.Power)
                            break;
                        total += Characteristics.Of(State, creature).Power;
                        Emit(new Tapped(creature.Id));
                    }
                    break;
                }
            }
        // What the costs took, for "the sacrificed creature's power", "the discarded card's mana value".
        bindings = (bindings ?? ImmutableDictionary<string, ImmutableList<TargetRef>>.Empty).SetItem("#cost", paid.ToImmutableList());

        // A mana ability doesn't use the stack; "spend this mana only to …" after it restricts what it made.
        if (effect is AddMana or LetVariable { Effect: AddMana })
        {
            Interpreter.Execute(effect, new EffectContext(this, player, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), Bindings: bindings.ToDictionary()));
            var abilities = Characteristics.AbilitiesOf(State, State.Object(source) ?? obj);
            if (index + 1 < abilities.Count && abilities[index + 1] is StaticAbility { Effect: ManaRestriction restriction })
                RestrictRecentMana(player, restriction, source);
            return;
        }

        var item = new StackItem(State.NextStackId, player, source, StackKind.Activated, [effect], targets, 0, Description: $"{obj.Card.Name}: {Interpreter.Describe(effect)}", Costs: ability.Costs.ToImmutableList(), AbilityIndex: index, Bindings: bindings, TargetCounts: targetCounts);
        Emit(new AbilityActivated(item));
    }

    /// <summary>Targets for each slot, chosen by <paramref name="player"/>; null when a required slot has no legal target.</summary>
    ImmutableList<TargetRef> ChooseTargetsFor(List<TargetSlot> slots, PlayerId player, ObjectId? source)
    {
        List<TargetRef> targets = [];
        List<int> counts = [];
        _lastTargetCounts = [];
        foreach (var slot in slots)
        {
            // An object can't be chosen twice among a spell's targets here (the lowering doesn't always say "another"); a player can.
            var candidates = TargetSlots.Candidates(slot, this, player, source).Where(x => x.IsPlayer || !targets.Contains(x)).ToImmutableList();
            var max = Math.Min(slot.Count, candidates.Count);
            var min = slot.UpTo ? 0 : Math.Min(1, max);
            if (candidates.Count == 0)
            {
                if (slot.UpTo)
                {
                    counts.Add(0);
                    continue;
                }
                return null;
            }
            var chosen = Decide(new ChooseTargets(player, "Choose a target", candidates, min, max)).Targets?.Where(candidates.Contains).Distinct().Take(max).ToList() ?? [];
            if (chosen.Count < min)
                chosen = candidates.Take(min).ToList();
            targets.AddRange(chosen);
            counts.Add(chosen.Count);
        }
        _lastTargetCounts = counts.ToImmutableList();
        return targets.ToImmutableList();
    }

    /// <summary>How many targets each slot took in the last <see cref="ChooseTargetsFor"/>, for the stack item made from it.</summary>
    ImmutableList<int> _lastTargetCounts = [];

    bool Pay(PlayerId player, ManaCost cost, bool convoke = false, ManaPayment.Purpose purpose = null)
    {
        var plan = ManaPayment.Plan(State, player, cost, convoke, purpose);
        if (plan is null)
            return false;

        foreach (var (source, color) in plan)
        {
            if (source.Delve)
            {
                MoveToZone(source.Id, Zone.Exile);
                Emit(new ManaAdded(player, color, 1));
                continue;
            }
            Emit(new Tapped(source.Id));
            Emit(new TappedForMana(source.Id, player));
            Emit(new ManaAdded(player, color, source.Amount));
        }

        var usable = ManaPayment.Usable(State, player, purpose);
        var remaining = usable.Pay(cost.SnowAsGeneric);
        if (remaining is null)
            return false;
        // What was spent, by color, comes out of the restricted mana that may pay for this first, then the rest of the pool.
        var spent = Colors.All.ToDictionary(c => c, c => usable.Of(c) - remaining.Of(c));
        var spentColorless = usable.Colorless - remaining.Colorless;
        _lastSpent = new ManaPool(spent.Where(x => x.Value > 0).ToImmutableDictionary(), spentColorless);
        var restricted = ImmutableList.CreateBuilder<RestrictedMana>();
        foreach (var mana in State.Player(player).RestrictedList)
        {
            var left = mana.Amount;
            if (ManaPayment.Allows(mana.Restriction, purpose, State, mana.Controller, mana.Source))
            {
                var due = mana.Color is { } c ? spent[c] : spentColorless;
                var take = Math.Min(due, left);
                left -= take;
                if (mana.Color is { } color)
                    spent[color] -= take;
                else
                    spentColorless -= take;
            }
            if (left > 0)
                restricted.Add(mana with { Amount = left });
        }
        var pool = State.Player(player).Pool;
        var after = new ManaPool(Colors.All.ToImmutableDictionary(c => c, c => Math.Max(0, pool.Of(c) - spent[c])), Math.Max(0, pool.Colorless - spentColorless));
        Emit(new ManaPaid(player, after, restricted.ToImmutable()));
        return true;
    }

    /// <summary>
    /// Restricts the mana <paramref name="player"/> was just given ("add {r}{r}{r}. spend this mana only to cast artifact
    /// spells"): the mana added since the last event that wasn't mana being added.
    /// </summary>
    public void RestrictRecentMana(PlayerId player, ManaRestriction restriction, ObjectId source)
    {
        var recent = _log.AsEnumerable().Reverse().TakeWhile(x => x is ManaAdded or EffectSkipped).OfType<ManaAdded>().Where(x => x.Player == player).ToList();
        foreach (var added in recent)
            Emit(new ManaRestricted(player, new RestrictedMana(added.Color, added.Amount, restriction, source, player)));
    }

    /// <summary>The mana the last payment spent, for "if {u} was spent to cast it".</summary>
    ManaPool _lastSpent = ManaPool.Empty;

    /// <summary>Resolves the stack to empty, checking state-based actions and triggers between items (for tools that set a game up by hand); false if it didn't empty within <paramref name="limit"/> items.</summary>
    public bool ResolveStack(int limit = 50)
    {
        for (var i = 0; i < limit; i++)
        {
            CheckStateAndTriggers();
            if (State.Stack.Count == 0)
                return true;
            ResolveTop();
        }
        return State.Stack.Count == 0;
    }

    void ResolveTop()
    {
        var item = State.Stack[^1];
        Emit(new ResolutionBegan(item.Id));

        // A bestowed aura whose creature is gone resolves as a creature instead.
        var bestowed = item.Spell is { } bestowId && State.Object(bestowId)?.WasCastWith("Bestowed") == true;
        if (item.Targets.Count > 0 && item.Targets.All(x => !TargetStillLegal(x)) && !bestowed)
        {
            Emit(new Fizzled(item.Id));
            if (item.Spell is { } fizzledSpell)
                MoveToZone(fizzledSpell, Zone.Graveyard);
            return;
        }

        if (item.Spell is { } spellId && State.Object(spellId) is { } spell && spell.Card.IsPermanentCard)
        {
            var host = spell.Card.IsAura || bestowed ? item.Targets.FirstOrDefault(x => x.IsObject && TargetStillLegal(x)).Object : null;
            MoveToZone(spellId, Zone.Battlefield, item.Controller, attachTo: host);
            Emit(new Resolved(item.Id));
            return;
        }

        var ctx = EffectContext.For(this, item, item.Subject);
        foreach (var effect in item.Effects)
            ctx = Interpreter.Execute(effect, ctx);
        if (item.Kind == StackKind.Triggered && item.DelayedId is { } delayedId)
            Emit(new DelayedTriggerRemoved(delayedId));

        if (item.Spell is { } cardId && State.Object(cardId) is { Zone: Zone.Stack } resolved)
        {
            // Rebound: cast from the hand, it's exiled as it resolves, and may be cast free at its owner's next upkeep.
            if (!resolved.IsToken && resolved.WasCastWith("FromHand") && resolved.Card.Keywords.Contains("Rebound"))
            {
                MoveToZone(cardId, Zone.Exile);
                var you = new PlayerSelector(PlayerKind.You);
                Emit(new DelayedTriggerCreated(new DelayedTrigger(State.NextDelayedId, new PhaseEvent(new PhaseRef("Upkeep", Whose: you, Next: true)),
                    new PlayCard(you, new Self(), Free: true), item.Controller, cardId, null, null)));
            }
            else
                MoveToZone(cardId, resolved.WasCastWith("Buyback") ? Zone.Hand : Zone.Graveyard);
        }

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
            UpdateControl();
            StartEngines();

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
                if (obj.Card.IsAura && (obj.AttachedTo is null || State.Object(obj.AttachedTo.Value) is not { Zone: Zone.Battlefield } enchanted
                    || Keywords.ProtectedFrom(State, enchanted, Characteristics.Of(State, enchanted), obj)))
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

        List<PendingTrigger> triggered = [.. StateTriggers()];
        if (pending.Count == 0 && triggered.Count == 0)
            return false;
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

                // The modes are chosen as it goes on the stack, then the chosen modes' targets.
                var (effect, bindings) = ResolveModes(trigger.Ability.Effect, controller, source: trigger.Source.Id);
                var slots = TargetSlots.Of([effect]).ToList();
                var targets = ChooseTargetsFor(slots, controller, trigger.Source.Id);
                if (targets is null)
                    continue;
                var targetCounts = _lastTargetCounts;
                var item = new StackItem(State.NextStackId, controller, trigger.Source.Zone == Zone.Command ? null : trigger.Source.Id, StackKind.Triggered, [effect], targets, 0,
                    Description: $"{trigger.Source.Card.Name} trigger", Subject: trigger.Subject, Bindings: bindings, DelayedId: trigger.Delayed?.Id, AbilityIndex: trigger.AbilityIndex, TargetCounts: targetCounts);
                if (trigger.Delayed is { } fired)
                    Emit(new DelayedTriggerRemoved(fired.Id));
                Emit(new Triggered(item));
                any = true;
            }

        // Putting triggers on the stack emits events of its own; those don't trigger anything, so drop them.
        _pending.Clear();
        return any;
    }

    /// <summary>
    /// State triggers whose condition holds now ("when this enchantment has no counters on it, sacrifice it"): each
    /// triggers once, and again only after that instance has left the stack.
    /// </summary>
    IEnumerable<PendingTrigger> StateTriggers()
    {
        foreach (var source in State.Battlefield.ToList())
        {
            var index = -1;
            foreach (var ability in Characteristics.AbilitiesOf(State, source))
            {
                index++;
                if (ability is not TriggeredAbility { Trigger: ConditionEvent { Condition: var condition } } trigger)
                    continue;
                if (State.Stack.Any(x => x.Kind == StackKind.Triggered && x.Source == source.Id && x.AbilityIndex == index))
                    continue;
                var ctx = new EffectContext(this, source.Controller, source.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
                if (Conditions.Evaluate(condition, ctx) == true)
                    yield return new PendingTrigger(source, trigger, null, source.Controller, AbilityIndex: index);
            }
        }
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
                    // "It escapes with …": only when it was cast with escape.
                    if (enters.States?.Contains("Escapes") == true && !obj.WasCastWith("Escaped"))
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
        // Sneak: a creature cast for its sneak cost enters tapped and attacking.
        if (obj.WasCastWith("Sneak") && obj.Card.IsCreature)
        {
            tapped = true;
            onEntry.Add(new EnterAttacking());
        }
        // "You may have this creature enter as a copy of any creature on the battlefield."
        foreach (var line in obj.Card.Abilities.OfType<EffectLine>())
            if (line.Effect is BecomeCopy { Verb: "EntersAs" } or Optional { Effect: BecomeCopy { Verb: "EntersAs" } })
                onEntry.Add(line.Effect);
        // "This creature enters with x +1/+1 counters on it, where x is …": the counters, with x known.
        foreach (var line in obj.Card.Abilities.OfType<EffectLine>())
            if (line.Effect is LetVariable { Effect: EntersBattlefield { Objects: null or Self, If: null, With: { Count: > 0 } counters } } let)
                onEntry.Add(let with { Effect = new Sequence(counters) });
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
            if (Dredged(player))
                continue;
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
        amount = Modify(source, target, amount, combat);
        amount = Prevent(source, target, amount, combat);
        if (amount <= 0)
            return;
        var dealer = source is { } s ? State.Object(s) : null;
        var traits = dealer is { Zone: Zone.Battlefield } ? Characteristics.Of(State, dealer) : null;
        var lifelink = traits?.HasKeyword("Lifelink") == true;
        var deathtouch = traits?.HasKeyword("Deathtouch") == true;
        var infect = traits?.HasKeyword("Infect") == true;
        var wither = infect || traits?.HasKeyword("Wither") == true;
        var toxic = traits?.Keywords.Where(x => x.StartsWith("Toxic:", StringComparison.Ordinal)).Sum(x => int.TryParse(x[6..], out var n) ? n : 0) ?? 0;

        if (target.IsPlayer)
        {
            if (State.Player(target.Player.Value).Lost)
                return;
            Emit(new DamageDealt(source, target, amount, combat, lifelink, deathtouch));
            if (infect)
                Emit(new PoisonChanged(target.Player.Value, amount));
            else
                Emit(new LifeChanged(target.Player.Value, -amount, "damage"));
            if (combat && toxic > 0)
                Emit(new PoisonChanged(target.Player.Value, toxic));
        }
        else
        {
            var victim = State.Object(target.Object.Value);
            if (victim is null || victim.Zone != Zone.Battlefield)
                return;
            var asCounters = wither && Characteristics.Of(State, victim).IsCreature;
            Emit(new DamageDealt(source, target, amount, combat, lifelink, deathtouch, asCounters));
            if (victim.Card.IsPlaneswalker)
                Emit(new CountersChanged(victim.Id, "loyalty", -amount));
            if (asCounters)
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
        if (TotemArmorSaves(id))
            return true;
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
        // Madness: the card is discarded into exile, and its owner may cast it for its madness cost; if not, it goes to the graveyard.
        if (State.Object(id) is { IsToken: false } discarded && KeywordCost(discarded.Card, "Madness") is { } madness)
        {
            MoveToZone(id, Zone.Exile);
            if (State.Object(id) is { Zone: Zone.Exile } exiled && AdditionalPayable(madness, player, id, 0) && TargetsAvailable(SpellSlots(exiled.Card), player, id)
                && Decide(new YesNo(player, $"Cast {exiled.Card.Name} for its madness cost?")).Yes)
                Cast(id, player, 0, alternative: MadnessCast);
            if (State.Object(id) is { Zone: Zone.Exile })
                MoveToZone(id, Zone.Graveyard);
            return;
        }
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
