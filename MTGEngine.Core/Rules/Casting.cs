namespace MTGEngine.Core;

/// <summary>
/// Casting a spell: the total cost (mana cost, X, kicker, additional costs, and cost changes from the spell itself
/// and from permanents), the choices made as it's cast, and its effects narrowed to how it was cast ("if this spell
/// was kicked") before its targets are chosen.
/// </summary>
public sealed partial class Game
{
    void Cast(ObjectId id, PlayerId player, int maxX, bool free = false, int alternative = -1)
    {
        var obj = State.Object(id);
        var card = obj.Card;
        var x = card.ManaCost.X > 0 && maxX > 0 && alternative == -1 ? Math.Clamp(Decide(new ChooseNumber(player, "Choose X", 0, maxX)).Number, 0, maxX) : 0;
        var mana = free || alternative >= 0 || alternative == Flashback ? ManaCost.Free : card.ManaCost.WithX(x);
        var options = ImmutableHashSet.CreateBuilder<string>();
        List<Effect> actions = [];

        if (alternative == FaceDownCast)
        {
            mana = ManaCost.Parse("{3}");
            options.Add("FaceDown");
        }

        if (alternative == Flashback && FlashbackCost(card) is { } flashback)
        {
            mana = ManaCost.Free;
            options.Add("Flashback");
            if (!PlanAdditional(flashback, player, id, 0, ref mana, actions, options))
                return;
        }

        // "You may exile two green cards from your hand rather than pay this spell's mana cost."
        if (alternative >= 0 && Alternatives(card).FirstOrDefault(a => a.Index == alternative).Alternative is { } chosen)
        {
            options.Add("Alternative");
            foreach (var part in new[] { chosen.Cost, chosen.Additional }.Where(c => c is not null))
                if (!PlanAdditional(part, player, id, 0, ref mana, actions, options))
                    return;
        }

        // Kicker: an optional additional mana cost, decided as the spell is cast.
        foreach (var kicker in Kickers(card))
            if (ManaPayment.CanPay(State, player, CostToCast(obj, player, mana.Plus(kicker), null)) && Decide(new YesNo(player, $"Pay kicker {kicker}?")).Yes)
            {
                mana = mana.Plus(kicker);
                options.Add("Kicked");
            }

        foreach (var buyback in OptionalCosts(card, "Buyback"))
            if (ManaPayment.CanPay(State, player, CostToCast(obj, player, mana.Plus(buyback), null)) && Decide(new YesNo(player, $"Pay buyback {buyback}?")).Yes)
            {
                mana = mana.Plus(buyback);
                options.Add("Buyback");
            }

        foreach (var cost in AdditionalCostsOf(card))
            if (!PlanAdditional(cost, player, id, x, ref mana, actions, options))
                return;

        List<Effect> effects = [];
        ImmutableDictionary<string, ImmutableList<TargetRef>> bindings = null;
        if (!card.IsPermanentCard)
            foreach (var spellEffect in card.SpellEffects)
            {
                if (Specialize(spellEffect, options) is not { } specialized)
                    continue;
                var (resolved, modeBindings) = ResolveModes(specialized, player);
                effects.Add(resolved);
                if (modeBindings is not null)
                    bindings = (bindings ?? ImmutableDictionary<string, ImmutableList<TargetRef>>.Empty).SetItems(modeBindings);
            }

        var slots = options.Contains("FaceDown") ? [] : card.IsPermanentCard ? SpellSlots(card) : TargetSlots.Of(effects).ToList();
        var targets = ChooseTargetsFor(slots, player, id);
        if (targets is null)
            return;

        // Cost changes are judged with the targets known ("costs {1} less if it targets a tapped creature").
        var bySelector = new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance);
        for (var i = 0; i < slots.Count && i < targets.Count; i++)
            bySelector[slots[i].Selector] = [targets[i]];
        var total = free || options.Contains("FaceDown") ? mana : CostToCast(obj, player, mana, bySelector);
        _lastSpent = ManaPool.Empty;
        if (!Pay(player, total, HasConvoke(card), new ManaPayment.Purpose(Spell: obj)))
            return;
        var spent = _lastSpent;
        options.Add("Cast");
        options.Add("From" + obj.Zone);

        MoveToZone(id, Zone.Stack, player);
        if (options.Contains("Flashback") && State.Object(id) is { } onStack)
            Emit(new ZoneReplacementCreated(new ZoneReplacement("LeaveTheStack", [(id, onStack.ZoneChangeCount)], Zone.Exile, null)));
        if (actions.Count > 0)
        {
            var ctx = new EffectContext(this, player, id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), x, new Dictionary<string, ImmutableList<TargetRef>>());
            List<TargetRef> paid = [];
            foreach (var action in actions)
            {
                ctx = Interpreter.Execute(action, ctx);
                if (ctx.Affected is { Count: > 0 } affected)
                {
                    paid.AddRange(affected);
                    if (BindingOf(action) is { } binding)
                        ctx.Binds[binding] = affected;
                }
            }
            ctx.Binds["#cost"] = paid.ToImmutableList();
            bindings = (bindings ?? ImmutableDictionary<string, ImmutableList<TargetRef>>.Empty).SetItems(ctx.Binds);
        }
        Emit(new CastOptionsChosen(id, options.ToImmutable(), spent));
        var item = new StackItem(State.NextStackId, player, id, StackKind.Spell, effects.ToImmutableList(), targets, x, Spell: id, Description: card.Name, Bindings: bindings);
        Emit(new SpellCast(item));
    }

    /// <summary>Whether <paramref name="player"/> could cast <paramref name="obj"/> now as far as its costs go: mana (with cost changes) and additional costs.</summary>
    bool CanAffordToCast(GameObject obj, PlayerId player) =>
        ManaPayment.CanPay(State, player, CostToCast(obj, player, obj.Card.ManaCost.WithX(0).Plus(RequiredAdditionalMana(obj.Card)), null), HasConvoke(obj.Card), new ManaPayment.Purpose(Spell: obj))
        && AdditionalCostsOf(obj.Card).All(cost => AdditionalPayable(cost, player, obj.Id, 0));

    /// <summary>Whether a spell has convoke: the keyword, or "this spell has convoke".</summary>
    static bool HasConvoke(CardDefinition card) =>
        card.Keywords.Contains("Convoke") || card.Abilities.Any(a => a is StaticAbility { Effect: Convoke { Spells: Self or null } });

    /// <summary>The ways to cast a card "rather than pay this spell's mana cost", with their conditions.</summary>
    static IEnumerable<(int Index, AlternativeCost Alternative)> Alternatives(CardDefinition card) =>
        card.Abilities.OfType<AlternativeCost>().Select((alternative, i) => (i, alternative)).Where(x => x.alternative.Replaces == "ThisSpellsManaCost");

    /// <summary>Whether <paramref name="player"/> could cast <paramref name="obj"/> now for an alternative cost: its condition holds and its costs can be paid.</summary>
    bool CanAffordAlternative(GameObject obj, PlayerId player, AlternativeCost alternative)
    {
        if (alternative.If is { } condition && Conditions.Evaluate(condition, new EffectContext(this, player, obj.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance))) != true)
            return false;
        var mana = new[] { alternative.Cost, alternative.Additional }.OfType<PayMana>().Aggregate(ManaCost.Free, (sum, m) => sum.Plus(ManaCost.Parse(string.Concat(m.Symbols))));
        return ManaPayment.CanPay(State, player, mana.Plus(RequiredAdditionalMana(obj.Card)), HasConvoke(obj.Card), new ManaPayment.Purpose(Spell: obj))
            && new[] { alternative.Cost, alternative.Additional }.Where(c => c is not null and not MTGRulesIR.Model.PayMana).All(c => AdditionalPayable(c, player, obj.Id, 0))
            && AdditionalCostsOf(obj.Card).All(cost => AdditionalPayable(cost, player, obj.Id, 0));
    }

    static IEnumerable<ManaCost> Kickers(CardDefinition card) => OptionalCosts(card, "Kicker");

    /// <summary>The mana of a card's optional extra cost keyword ("kicker {4}", "buyback {3}").</summary>
    static IEnumerable<ManaCost> OptionalCosts(CardDefinition card, string keyword) =>
        card.Abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords)
            .Where(x => CardDefinition.Normalize(x.Keyword) == keyword && x.Cost is PayMana)
            .Select(x => ManaCost.Parse(string.Concat(((PayMana)x.Cost).Symbols)));

    static IEnumerable<Cost> AdditionalCostsOf(CardDefinition card) =>
        card.Abilities.OfType<AdditionalCost>().Where(x => x.Spells is Self).Select(x => x.Cost);

    static ManaCost RequiredAdditionalMana(CardDefinition card) =>
        AdditionalCostsOf(card).OfType<PayMana>().Aggregate(ManaCost.Free, (sum, mana) => sum.Plus(ManaCost.Parse(string.Concat(mana.Symbols))));

    /// <summary>
    /// The mana a spell costs <paramref name="player"/> after cost changes: "this spell costs {1} less for each …"
    /// on the spell, and "spells you cast cost {1} less / your opponents cast cost {1} more" on permanents.
    /// Reductions take generic mana only.
    /// </summary>
    public ManaCost CostToCast(GameObject spell, PlayerId player, ManaCost cost, Dictionary<IrNode, ImmutableList<TargetRef>> targets)
    {
        var asSpell = spell with { Zone = Zone.Stack, Controller = player };
        var reduction = 0;
        var state = State;

        void Apply(CostModification modification, EffectContext ctx)
        {
            if (modification.Action is not (null or "Cast"))
                return;
            if (modification.Condition is { } condition && Conditions.Evaluate(condition, ctx) != true)
                return;
            var times = modification.PerEach is null ? 1 : Quantities.Evaluate(modification.PerEach, ctx) ?? 0;
            if (times <= 0)
                return;
            var amount = ManaCost.Parse(string.Concat(modification.Amount?.Symbols ?? []));
            for (var i = 0; i < times; i++)
                if (modification.Change == "Less")
                    reduction += amount.Generic;
                else if (modification.Change == "More")
                    cost = cost.Plus(amount);
        }

        // The spell's own cost changes, read where it is (hand, graveyard, …) as if it were the spell.
        var own = new EffectContext(this, player, spell.Id, targets ?? new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
        foreach (var ability in spell.Card.Abilities)
            foreach (var (modification, condition, variable) in CostModifications(ability))
                if (modification.Spells is Self)
                {
                    var ctx = variable is { } let && Quantities.Evaluate(let.Definition, own) is { } value ? own with { X = value } : own;
                    if (condition is null || Conditions.Evaluate(condition, ctx) == true)
                        Apply(modification, ctx);
                }

        foreach (var source in state.Battlefield)
        {
            var traits = Characteristics.Of(state, source);
            if (traits.LostAbilities)
                continue;
            foreach (var ability in traits.Abilities.OfType<StaticAbility>())
                foreach (var (modification, condition, _) in CostModifications(ability))
                {
                    if (modification.Spells is not ObjectSelector selector)
                        continue;
                    var ctx = new EffectContext(this, source.Controller, source.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
                    if (condition is not null && Conditions.Evaluate(condition, ctx) != true)
                        continue;
                    if (!Filters.Matches(state, selector.Filter, asSpell, source.Controller, source.Id, selector.Other ? source.Id : null, ctx: ctx))
                        continue;
                    Apply(modification, ctx);
                }
        }

        return cost.AdjustGeneric(-reduction);
    }

    /// <summary>The cost changes an ability states, each with the condition it's under and the variable it defines ("where X is …").</summary>
    public static IEnumerable<(CostModification Modification, Condition Condition, LetVariable Variable)> CostModifications(Ability ability)
    {
        var (effect, condition) = CardDefinition.Unwrap(ability) switch
        {
            StaticAbility statics => (statics.Effect, statics.While),
            SpellAbility spell => (spell.Effect, null),
            EffectLine line => (line.Effect, null),
            _ => ((Effect)null, (Condition)null),
        };
        return Walk(effect, condition, null);

        static IEnumerable<(CostModification, Condition, LetVariable)> Walk(Effect effect, Condition condition, LetVariable variable) =>
            effect switch
            {
                CostModification modification => [(modification, condition, variable)],
                Conditional { Else: null } conditional => Walk(conditional.Then, condition is null ? conditional.If : new And([condition, conditional.If]), variable),
                LetVariable let => Walk(let.Effect, condition, let),
                Sequence sequence => sequence.Steps.SelectMany(x => Walk(x, condition, variable)),
                _ => [],
            };
    }

    /// <summary>Plans one additional cost: mana joins the spell's cost; other costs become actions done as it's cast. False when it can't be paid.</summary>
    bool PlanAdditional(Cost cost, PlayerId player, ObjectId spell, int x, ref ManaCost mana, List<Effect> actions, ImmutableHashSet<string>.Builder options)
    {
        switch (cost)
        {
            case PayMana symbols:
                mana = mana.Plus(ManaCost.Parse(string.Concat(symbols.Symbols)));
                return true;
            case PayLife life:
                actions.Add(new PayCost(life));
                return true;
            case CompoundCost compound:
                foreach (var part in compound.Parts)
                    if (!PlanAdditional(part, player, spell, x, ref mana, actions, options))
                        return false;
                return true;
            case CostChoice choice:
                return PlanChoice(choice.Options, player, spell, x, ref mana, actions, options);
            case ActionCost { Action: ChoiceOf choice }:
                return PlanChoice(choice.Options.Select(o => (Cost)new ActionCost(o)).ToList(), player, spell, x, ref mana, actions, options);
            case ActionCost { Action: Optional optional }:
            {
                var inner = optional.Effect is PayCost pay ? pay.Cost : new ActionCost(optional.Effect);
                if (!AdditionalPayable(inner, player, spell, x) || !Decide(new YesNo(player, $"Pay the additional cost: {Interpreter.Describe(optional.Effect)}?")).Yes)
                    return true;
                options.Add("AdditionalCost");
                return PlanAdditional(inner, player, spell, x, ref mana, actions, options);
            }
            case ActionCost { Action: PayCost pay }:
                return PlanAdditional(pay.Cost, player, spell, x, ref mana, actions, options);
            case ActionCost action:
                if (!AdditionalPayable(cost, player, spell, x))
                    return false;
                actions.Add(action.Action);
                options.Add("AdditionalCost");
                return true;
            default:
                Skip(cost, "additional cost kind");
                return true;
        }
    }

    bool PlanChoice(IReadOnlyList<Cost> choices, PlayerId player, ObjectId spell, int x, ref ManaCost mana, List<Effect> actions, ImmutableHashSet<string>.Builder options)
    {
        var payable = choices.Where(c => AdditionalPayable(c, player, spell, x)).ToList();
        if (payable.Count == 0)
            return false;
        var pick = payable.Count == 1 ? 0 : Decide(new ChooseOne(player, "Choose an additional cost", payable.Select(Interpreter.Describe).ToImmutableList())).Index;
        return PlanAdditional(payable[Math.Clamp(pick, 0, payable.Count - 1)], player, spell, x, ref mana, actions, options);
    }

    /// <summary>Whether an additional cost can be paid now: enough to sacrifice, discard, exile, reveal or tap, enough life.</summary>
    bool AdditionalPayable(Cost cost, PlayerId player, ObjectId spell, int x)
    {
        var ctx = new EffectContext(this, player, spell, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), x);
        switch (cost)
        {
            case PayMana symbols:
                return ManaPayment.CanPay(State, player, ManaCost.Parse(string.Concat(symbols.Symbols)));
            case PayLife life:
                return State.Player(player).Life >= (Quantities.Evaluate(life.Amount, ctx) ?? 0);
            case CompoundCost compound:
                return compound.Parts.All(p => AdditionalPayable(p, player, spell, x));
            case CostChoice choice:
                return choice.Options.Any(p => AdditionalPayable(p, player, spell, x));
            case ActionCost { Action: var action }:
                return ActionPayable(action, player, spell, x);
            default:
                return false;
        }
    }

    /// <summary>The action costs the engine can pay: what <see cref="ActionPayable"/> knows how to judge.</summary>
    public static readonly ImmutableHashSet<Type> PayableActions =
    [
        typeof(Optional), typeof(PayCost), typeof(ChoiceOf), typeof(Sequence), typeof(Sacrifice), typeof(Discard), typeof(Exile), typeof(Reveal), typeof(Tap),
        typeof(Untap), typeof(ChangeZone), typeof(PutCounters), typeof(RemoveCounters), typeof(Mill), typeof(ChooseEffect),
    ];

    /// <summary>Whether <paramref name="player"/> can do <paramref name="action"/> as a cost now: enough to sacrifice, discard, exile, reveal, tap, return, mill, or counters to remove.</summary>
    public bool ActionPayable(Effect action, PlayerId player, ObjectId? source, int x)
    {
        var spell = source ?? default;
        var ctx = new EffectContext(this, player, source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance), x);
        int Count(Quantity quantity) => quantity is Special { Kind: "AnyNumber" } ? 0 : Quantities.Evaluate(quantity, ctx) ?? 1;
        int Matching(ObjectSelector objects, Func<GameObject, bool> also = null) =>
            Resolver.Candidates(objects.Filter, ctx).Count(o => o.Id != spell && Filters.Matches(State, objects.Filter, o, player, source, objects.Other ? source : null, ctx: ctx) && (also?.Invoke(o) ?? true));
        int Wanted(ObjectSelector objects) =>
            objects.Count is Special { Kind: "AnyNumber" } ? 0 : objects.Quantifier == Quantifier.Count ? Count(objects.Count) : 1;
        var self = source is { } id ? State.Object(id) : null;

        switch (action)
        {
                    case Optional:
                        return true;
                    case PayCost pay:
                        return AdditionalPayable(pay.Cost, player, spell, x);
                    case ChoiceOf choice:
                        return choice.Options.Any(o => ActionPayable(o, player, source, x));
                    case Sequence sequence:
                        return sequence.Steps.All(s => ActionPayable(s, player, source, x));
                    case Sacrifice { Objects: ObjectSelector objects }:
                        return Matching(objects, o => o.Controller == player && o.Zone == Zone.Battlefield) >= Wanted(objects);
                    case Discard discard:
                        return State.Player(player).Hand.Count(h => h != spell && (discard.Filter is null || State.Object(h) is { } card && Resolver.PoolMatches(discard.Filter, card, ctx)))
                            >= (discard.Count is Special { Kind: "All" } ? 0 : Count(discard.Count));
                    case Exile { Objects: ObjectSelector objects }:
                        return Matching(objects) >= Wanted(objects);
                    case Reveal { What: ObjectSelector objects }:
                        return Matching(objects) >= Wanted(objects);
                    case Tap { Objects: ObjectSelector objects }:
                        return Matching(objects, o => !o.Tapped) >= Wanted(objects);
                    case ChangeZone { Objects: ObjectSelector objects }:
                        return Matching(objects) >= Wanted(objects);
                    case PutCounters { Objects: ObjectSelector objects }:
                        return Matching(objects) >= 1;
                    case ChooseEffect:
                        return true;
                    case Sacrifice { Objects: Self }:
                        return self is { Zone: Zone.Battlefield };
                    case Tap { Objects: Self }:
                        return self is { Zone: Zone.Battlefield, Tapped: false };
                    case Untap { Objects: Self }:
                        return self is { Zone: Zone.Battlefield, Tapped: true };
                    case Untap { Objects: ObjectSelector objects }:
                        return Matching(objects, o => o.Tapped) >= Wanted(objects);
                    case Exile { Objects: Self } or ChangeZone { Objects: Self } or Reveal { What: Self }:
                        return self is not null;
                    case Mill mill:
                        return State.Player(player).Library.Count >= Count(mill.Count);
                    case RemoveCounters { Objects: null or Self } remove:
                        return self is not null && (remove.Counters.Kind is { } kind ? self.CountersOf(kind) : self.CounterMap.Values.Sum()) >= (remove.Counters.Count is Special { Kind: "All" } ? 0 : Count(remove.Counters.Count));
                    case RemoveCounters { Objects: ObjectSelector objects } remove:
                        return Matching(objects, o => remove.Counters.Kind is { } kind ? o.CountersOf(kind) > 0 : o.CounterMap.Count > 0) >= 1;
                    case PutCounters { Objects: null or Self }:
                        return self is not null;
                    default:
                        return false;
        }
    }

    static string BindingOf(Effect action) =>
        action switch
        {
            Sacrifice { Objects: ObjectSelector { Binding: { } binding } } => binding,
            Exile { Objects: ObjectSelector { Binding: { } binding } } => binding,
            ChangeZone { Objects: ObjectSelector { Binding: { } binding } } => binding,
            Tap { Objects: ObjectSelector { Binding: { } binding } } => binding,
            _ => null,
        };

    /// <summary>
    /// An effect narrowed to how its spell was cast: "if this spell was kicked, …" becomes what it says or nothing.
    /// Null when nothing is left.
    /// </summary>
    public static Effect Specialize(Effect effect, ICollection<string> options)
    {
        switch (effect)
        {
            case Conditional { If: WasCastWith { Spell: Self, Twice: false } cast } conditional:
                return Specialize(options.Contains(cast.How) != cast.Negated ? conditional.Then : conditional.Else, options);
            case Conditional conditional:
                return conditional with { Then = Specialize(conditional.Then, options) ?? new Sequence([]), Else = conditional.Else is null ? null : Specialize(conditional.Else, options) };
            case Sequence sequence:
                return new Sequence(sequence.Steps.Select(step => Specialize(step, options)).Where(x => x is not null).ToList());
            default:
                return effect;
        }
    }
}
