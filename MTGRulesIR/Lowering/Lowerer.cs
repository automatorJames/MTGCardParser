using System.Collections;
using System.Collections.Concurrent;

namespace MTGRulesIR.Lowering;

/// <summary>
/// Lowers hydrated glyphs to the IR (see <see cref="IrNode"/>): a rule per glyph type that has one, and for the rest
/// the structure the primitives imply - a one-of lowers to its alternative, a dynamic to what it resolved to, a
/// wrapper to its items, a back-reference to a <see cref="Reference"/>, a vocabulary member to a <see cref="Term"/> -
/// down to an opaque node for a glyph with no rule yet. So every glyph lowers to something, and what stays opaque
/// names the next rule to write.
/// </summary>
public sealed partial class Lowerer
{
    readonly Dictionary<Type, Func<object, LoweringContext, IrNode>> _rules = [];

    public Lowerer()
    {
        RegisterFrames();
        RegisterSelectors();
        RegisterEffects();
        RegisterValues();
        RegisterRoundTwo();
        RegisterRoundThree();
        RegisterRoundFour();
        RegisterRoundFive();
        RegisterRoundSix();
        RegisterRoundSeven();
        RegisterRoundEight();
        RegisterRoundNine();
        RegisterRoundTen();
        RegisterRoundEleven();
    }

    /// <summary>The glyph (and vocabulary) types that have a lowering rule.</summary>
    public IReadOnlyCollection<Type> RuledTypes => _rules.Keys;

    void Rule<T>(Func<T, LoweringContext, IrNode> rule) =>
        _rules[typeof(T)] = (value, ctx) => rule((T)value, ctx);

    /// <summary>Lowers <paramref name="value"/> - a glyph, a vocabulary member, a primitive - to a node, or null for something that means nothing to the engine (reminder text).</summary>
    public IrNode Lower(object value, LoweringContext ctx)
    {
        if (value is null)
            return null;

        if (value is IrNode node)
            return node;

        var type = value.GetType();

        if (_rules.TryGetValue(type, out var rule))
        {
            var lowered = Bind(value, rule(value, ctx), ctx);
            ctx.Record(lowered, value);
            return lowered;
        }

        switch (value)
        {
            case OneOfBase oneOf:
                return Lower(oneOf.GetResolvedValue(), ctx);

            case DynamicGlyph dynamic:
                return Lower(dynamic.Item, ctx);

            case BackReference reference:
                var resolved = ctx.Reference(reference);
                ctx.Record(resolved, reference);
                return resolved;

            case Enum member:
                return Kind(member);

            case bool or int or string:
                return new Term(value.ToString(), type.Name);

            case Glyph glyph when IsWrapper(type):
                var combined = Combine(LowerAll(glyph, ctx), ctx.TextOf(glyph), type);
                ctx.Record(combined, glyph);
                return combined;

            case Glyph glyph:
                var opaque = new OpaqueNode(ctx.TextOf(glyph), type.Name);
                ctx.Record(opaque, glyph);
                return opaque;

            case IEnumerable list:
                return Combine(LowerAll(list, ctx), null, type);

            default:
                return new OpaqueNode(value.ToString(), type.Name);
        }
    }

    /// <summary>Lowers <paramref name="value"/> to the nodes it holds: a wrapper's or list's items each lowered, a single value as one node, nulls dropped.</summary>
    public List<IrNode> LowerAll(object value, LoweringContext ctx)
    {
        List<IrNode> nodes = [];
        Collect(value, ctx, nodes);
        return nodes;
    }

    void Collect(object value, LoweringContext ctx, List<IrNode> into)
    {
        switch (value)
        {
            case null:
                return;

            case IrNode node:
                into.Add(node);
                return;

            case string text:
                into.Add(new Term(text, "string"));
                return;

            case OneOfBase oneOf when !_rules.ContainsKey(oneOf.GetType()):
                Collect(oneOf.GetResolvedValue(), ctx, into);
                return;

            case DynamicGlyph dynamic:
                Collect(dynamic.Item, ctx, into);
                return;

            case Glyph glyph when IsWrapper(glyph.GetType()) && !_rules.ContainsKey(glyph.GetType()):
                foreach (var item in WrappedItems(glyph))
                    Collect(item, ctx, into);
                return;

            case IEnumerable list:
                foreach (var item in list)
                    Collect(item, ctx, into);
                return;

            default:
                if (Lower(value, ctx) is { } lowered)
                    into.Add(lowered);
                return;
        }
    }

    /// <summary>Lowers <paramref name="value"/> into the family <typeparamref name="T"/>: the node when it's one, else that family's opaque node holding the text.</summary>
    public T LowerAs<T>(object value, LoweringContext ctx) where T : IrNode =>
        Coerce<T>(Lower(value, ctx), value, ctx);

    /// <summary>Every node of <paramref name="value"/> (see <see cref="LowerAll"/>), each coerced into <typeparamref name="T"/>.</summary>
    public List<T> LowerAllAs<T>(object value, LoweringContext ctx) where T : IrNode =>
        LowerAll(value, ctx).Select(x => Coerce<T>(x, value, ctx)).ToList();

    T Coerce<T>(IrNode node, object source, LoweringContext ctx) where T : IrNode =>
        node switch
        {
            null => null,
            T typed => typed,
            KeywordLine line when typeof(T) == typeof(Effect) => (T)(IrNode)new KeywordAction(line.Keywords, line.Other),
            LabeledAbility { Ability: EffectLine line } when typeof(T) == typeof(Effect) => (T)(IrNode)line.Effect,
            LabeledAbility { Ability: StaticAbility { While: null } statics } when typeof(T) == typeof(Effect) => (T)(IrNode)statics.Effect,
            LabeledAbility { Ability: SpellAbility spell } when typeof(T) == typeof(Effect) => (T)(IrNode)spell.Effect,
            LabeledAbility { Ability: ReplacementAbility replacement } when typeof(T) == typeof(Effect) => (T)(IrNode)replacement.Effect,
            FlavorAbility { Ability: EffectLine line } when typeof(T) == typeof(Effect) => (T)(IrNode)line.Effect,
            FlavorAbility { Ability: StaticAbility { While: null } statics } when typeof(T) == typeof(Effect) => (T)(IrNode)statics.Effect,
            FlavorAbility { Ability: SpellAbility spell } when typeof(T) == typeof(Effect) => (T)(IrNode)spell.Effect,
            FlavorAbility { Ability: ReplacementAbility replacement } when typeof(T) == typeof(Effect) => (T)(IrNode)replacement.Effect,
            TriggeredAbility { If: null } trigger when typeof(T) == typeof(Effect) => (T)(IrNode)new Delayed(trigger.Trigger, trigger.Effect),
            FlavorAbility { Ability: TriggeredAbility { If: null } trigger } when typeof(T) == typeof(Effect) => (T)(IrNode)new Delayed(trigger.Trigger, trigger.Effect),
            FlavorAbility { Ability: StaticAbility { While: { } held } statics } when typeof(T) == typeof(Effect) => (T)(IrNode)new Lasting(statics.Effect, new While(held)),
            LabeledAbility { Ability: TriggeredAbility { If: null } trigger } when typeof(T) == typeof(Effect) => (T)(IrNode)new Delayed(trigger.Trigger, trigger.Effect),
            While lasting when typeof(T) == typeof(Condition) => (T)(IrNode)lasting.Condition,
            Scaled { Base: LifeTotal } scaled when typeof(T) == typeof(Cost) => (T)(IrNode)new PayLife(scaled),
            Effect effect when typeof(T) == typeof(Cost) => (T)(IrNode)new ActionCost(effect),
            KeywordInstance keyword when typeof(T) == typeof(Cost) => (T)(IrNode)new KeywordCost(keyword),
            BuffGroup group when typeof(T) == typeof(Effect) => (T)(IrNode)PumpOf(group, null, null, ctx),
            ObjectEvent happening when typeof(T) == typeof(Condition) => (T)(IrNode)new Happened(happening, null),
            Happened { Event: ObjectEvent inner } happened when typeof(T) == typeof(Event) => (T)(IrNode)(inner with { Only = inner.Only ?? happened.Span }),
            Happened happened when typeof(T) == typeof(Event) => (T)(IrNode)happened.Event,
            Term term when typeof(T) == typeof(Selector) && term.Vocabulary is nameof(G.SpellReference) or "Its" or "PersonalPronoun" or "ThatPlayer" => (T)(IrNode)new Reference(null, term.Value),
            Term { Vocabulary: "String" or "string", Value: "its" or "that" or "it" or "them" or "they" } term when typeof(T) == typeof(Selector) => (T)(IrNode)new Reference(null, term.Value),
            Term { Vocabulary: "String" or "string", Value: "lands" } when typeof(T) == typeof(Selector) => (T)(IrNode)new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term(nameof(G.CardType.Land), nameof(G.CardType))])),
            Term { Vocabulary: "String" or "string", Value: "that many" } when typeof(T) == typeof(Quantity) => (T)(IrNode)new Special("ThatMany"),
            Term { Vocabulary: "String" or "string", Value: "saddled" } when typeof(T) == typeof(Condition) => (T)(IrNode)new ObjectIs(ctx.Subject ?? new Self(), [new Term("Saddled", nameof(G.PermanentDesignation))]),
            Term { Vocabulary: nameof(G.CounterSet), Value: nameof(G.CounterSet.ThatMany) } when typeof(T) == typeof(Quantity) => (T)(IrNode)new Special("ThatMany"),
            Term { Vocabulary: nameof(G.CounterSet) } when typeof(T) == typeof(Quantity) => (T)(IrNode)new Special("All"),
            Term { Vocabulary: nameof(G.CounterSet) } term when typeof(T) == typeof(Selector) => (T)(IrNode)new Reference(null, term.Value.ToLowerInvariant()),
            Term { Vocabulary: nameof(G.DividedRecipients) } term when typeof(T) == typeof(Selector) => (T)(IrNode)new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [term.Value == nameof(G.DividedRecipients.TargetCreatures) ? new Term(nameof(G.CardType.Creature), nameof(G.CardType)) : new Term(nameof(ObjectKind.Any), nameof(ObjectKind))])),
            AlternativeCost alternative when typeof(T) == typeof(Effect) => (T)(IrNode)new AlternativeCostOffer(alternative),
            Effect effect when typeof(T) == typeof(Event) => (T)(IrNode)new EffectEvent(effect),
            Effect effect when typeof(T) == typeof(Condition) => (T)(IrNode)new Did(effect),
            ObjectIs aspect when typeof(T) == typeof(Effect) => (T)(IrNode)new BecomesAspect(aspect.Object, aspect.Aspects, Negated: aspect.Negated, Verb: aspect.Tense),
            Effect effect when typeof(T) == typeof(Ability) => (T)(IrNode)AsAbility(effect, ctx, source),
            ObjectFilter filter when typeof(T) == typeof(Selector) => (T)(IrNode)new ObjectSelector(Quantifier.Any, filter),
            PhaseRef phase when typeof(T) == typeof(Event) => (T)(IrNode)new PhaseEvent(phase),
            PhaseRef phase when typeof(T) == typeof(Duration) => (T)(IrNode)new UntilPhase(phase),
            Condition condition when typeof(T) == typeof(Duration) => (T)(IrNode)new While(condition),
            Condition condition when typeof(T) == typeof(Event) => (T)(IrNode)new ConditionEvent(condition),
            IOpaque opaque => (T)Opaque(typeof(T), opaque.Text, opaque.Glyph),
            Term { Vocabulary: nameof(G.CardType) or nameof(G.PermanentKind) or nameof(G.CreatureType) or nameof(G.LandType) } term when typeof(T) == typeof(Selector) => (T)(IrNode)new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [term])),
            Term { Vocabulary: nameof(G.PermanentDesignation) or nameof(G.TapState) or nameof(G.CombatState) } term when typeof(T) == typeof(Condition) => (T)(IrNode)new ObjectIs(ctx.Subject ?? new Self(), [term]),
            Term term => (T)Opaque(typeof(T), ctx.TextOf(source) ?? term.Value, $"{source?.GetType().Name}→Term[{term.Vocabulary}] as {typeof(T).Name}"),
            _ => (T)Opaque(typeof(T), ctx.TextOf(source) ?? node.ToString(), $"{source?.GetType().Name}→{node.GetType().Name} as {typeof(T).Name}"),
        };

    /// <summary>
    /// A lowered node as an ability: a frame as it is; an effect as the spell's effect on an instant or sorcery, as
    /// a replacement when it says how the permanent enters or what happens as an event does, as a static ability
    /// when it's continuous (a buff with no end, or one that holds while a condition does), else an effect line
    /// still to be classified. Anything else is opaque.
    /// </summary>
    public Ability AsAbility(IrNode node, LoweringContext ctx, object glyph) =>
        node switch
        {
            null => null,
            Ability ability => ability,
            EntersBattlefield or AsEventHappens or Replacement => new ReplacementAbility((Effect)node),
            Effect effect when IsSpell(ctx.Types) => new SpellAbility(effect),
            Pump { Duration: While lasting } pump => new StaticAbility(pump with { Duration = null }, lasting.Condition),
            Lasting { Duration: While lasting } wrapped => new StaticAbility(wrapped.Effect, lasting.Condition),
            Effect effect when IsContinuous(effect) => new StaticAbility(effect),
            Effect effect => new EffectLine(effect),
            IOpaque opaque => new OpaqueAbility(opaque.Text, opaque.Glyph),
            _ => new OpaqueAbility(ctx.TextOf(glyph) ?? node.ToString(), $"{glyph?.GetType().Name}→{node.GetType().Name}"),
        };

    static bool IsSpell(string types) =>
        types.Contains("Instant") || types.Contains("Sorcery");

    static bool IsContinuous(Effect effect) =>
        effect switch
        {
            Pump { Duration: null } => true,
            LoseAllAbilities => true,
            CantBeActivated => true,
            MustBlock { Duration: null } => true,
            MustBeBlocked { Duration: null } => true,
            AssignsNoCombatDamage { Duration: null } => true,
            CantGainLife { Duration: null } => true,
            DamageCantBePrevented { Which: null, Duration: null } => true,
            ExchangeControl => false,
            SameIsTrueFor => true,
            PlayWithTopCardRevealed => true,
            KeywordCostIsManaCost => true,
            BeginGameOnBattlefield => true,
            SpendManaAsThough => true,
            CrewsAsThoughPowerGreater => true,
            NoMaximumHandSize { Duration: null } => true,
            CanBlock => true,
            CantPlay { Duration: null } => true,
            ExtraLandPlays { Duration: null } => true,
            Convoke => true,
            GameOutcome { ThisTurn: false } => true,
            CastAsThoughFlash { Duration: null } => true,
            MayNotUntap => true,
            CantBeTargeted { Duration: null } => true,
            CombatRestriction { Duration: null } => true,
            CostModification => true,
            DoesntUntap => true,
            GrantAbility => true,
            Animate { Duration: null } => true,
            BecomesAspect { Duration: null } => true,
            SetStats => true,
            MayPlayFrom { Duration: null } => true,
            ManaRestriction => true,
            Sequence sequence => sequence.Steps.All(IsContinuous),
            _ => false,
        };

    /// <summary>The opaque node of <paramref name="family"/> (an abstract IR family) holding <paramref name="text"/> from glyph type <paramref name="glyph"/>.</summary>
    public static IrNode Opaque(Type family, string text, string glyph) =>
        family == typeof(Ability) ? new OpaqueAbility(text, glyph)
        : family == typeof(Effect) ? new OpaqueEffect(text, glyph)
        : family == typeof(Selector) ? new OpaqueSelector(text, glyph)
        : family == typeof(Quantity) ? new OpaqueQuantity(text, glyph)
        : family == typeof(Duration) ? new OpaqueDuration(text, glyph)
        : family == typeof(Condition) ? new OpaqueCondition(text, glyph)
        : family == typeof(Event) ? new OpaqueEvent(text, glyph)
        : family == typeof(Cost) ? new OpaqueCost(text, glyph)
        : new OpaqueNode(text, glyph);

    /// <summary>Several nodes where one is wanted: selectors join as a <see cref="Union"/>, effects as a <see cref="Sequence"/>, conditions as an <see cref="And"/>; anything mixed stays opaque.</summary>
    static IrNode Combine(List<IrNode> nodes, string text, Type type) =>
        nodes.Count == 0 ? null
        : nodes.Count == 1 ? nodes[0]
        : nodes.All(x => x is Selector) ? new Union(nodes.Cast<Selector>().ToList())
        : nodes.All(x => x is Effect) ? new Sequence(nodes.Cast<Effect>().ToList())
        : nodes.All(x => x is Condition) ? new And(nodes.Cast<Condition>().ToList())
        : new OpaqueNode(text ?? string.Join(", ", nodes.Select(x => x.ToString())), FriendlyName(type));

    /// <summary>Gives a selector lowered from a glyph that introduces a referent its binding, so later references can name it.</summary>
    static IrNode Bind(object glyph, IrNode node, LoweringContext ctx)
    {
        if (node is null || !HasReferent(glyph.GetType()))
            return node;

        var id = ctx.Bind(glyph);

        return node switch
        {
            ObjectSelector { Binding: null } selector => selector with { Binding = id },
            PlayerSelector { Binding: null } player => player with { Binding = id },
            _ => node,
        };
    }

    static readonly ConcurrentDictionary<Type, bool> _hasReferent = new();

    static bool HasReferent(Type type) =>
        _hasReferent.GetOrAdd(type, x => x.IsDefined(typeof(ReferentAttribute)) || x.GetProperties().Any(p => p.IsDefined(typeof(ReferentAttribute))));

    /// <summary>Whether <paramref name="type"/> only wraps items: a <see cref="ManyOf{T}"/>, <see cref="OptionalOf{T}"/> or any compound.</summary>
    public static bool IsWrapper(Type type) =>
        type.IsAssignableTo(typeof(CompoundOfBase))
        || type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(ManyOf<>) || type.GetGenericTypeDefinition() == typeof(OptionalOf<>));

    /// <summary>Whether <paramref name="type"/> holds its contents rather than owning them: a wrapper, a one-of or a dynamic.</summary>
    public static bool IsTransparent(Type type) =>
        IsWrapper(type) || type.IsAssignableTo(typeof(OneOfBase)) || type.IsAssignableTo(typeof(DynamicGlyph));

    static IEnumerable<object> WrappedItems(Glyph wrapper)
    {
        var type = wrapper.GetType();

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(OptionalOf<>))
            return type.GetProperty("Item")?.GetValue(wrapper) is { } item ? [item] : [];

        return type.GetProperty("Items")?.GetValue(wrapper) is IEnumerable items ? items.Cast<object>().Where(x => x is not null) : [];
    }

    static string FriendlyName(Type type) =>
        type.IsGenericType
            ? $"{type.Name[..type.Name.IndexOf('`')]}<{string.Join(", ", type.GetGenericArguments().Select(FriendlyName))}>"
            : type.Name;
}
