namespace MTGRulesIR.Lowering;

public sealed partial class Lowerer
{
    static readonly Dictionary<string, int> _numberWords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Zero"] = 0, ["One"] = 1, ["Two"] = 2, ["Three"] = 3, ["Four"] = 4, ["Five"] = 5, ["Six"] = 6, ["Seven"] = 7,
        ["Eight"] = 8, ["Nine"] = 9, ["Ten"] = 10, ["Eleven"] = 11, ["Twelve"] = 12, ["Thirteen"] = 13, ["Fourteen"] = 14,
        ["Fifteen"] = 15, ["Sixteen"] = 16, ["Seventeen"] = 17, ["Eighteen"] = 18, ["Nineteen"] = 19, ["Twenty"] = 20,
    };

    static Quantity QuantityOf(G.Quantity quantity) =>
        quantity == G.Quantity.X ? new Variable("X")
        : _numberWords.TryGetValue(quantity.ToString(), out var number) ? new Fixed(number)
        : new Special(quantity.ToString());

    static Quantity Signed(G.PlusMinus sign, Quantity quantity) =>
        sign != G.PlusMinus.Minus ? quantity
        : quantity is Fixed fixedValue ? new Fixed(-fixedValue.Value)
        : new Negated(quantity);

    /// <summary>The value of <paramref name="glyph"/>'s property <paramref name="name"/>, for a glyph type read by name rather than compiled against.</summary>
    static object Property(object glyph, string name) =>
        glyph?.GetType().GetProperty(name)?.GetValue(glyph);

    static Quantity Multiply(Quantity amount, Quantity perEach) =>
        perEach is null ? amount : new Product(amount ?? new Fixed(1), perEach);

    /// <summary>The clause's subject, or "you" in an imperative ("draw a card").</summary>
    static Selector Actor(LoweringContext ctx) =>
        ctx.Subject ?? new PlayerSelector(PlayerKind.You);

    static PlayerSelector Player(G.WhichPlayer player) =>
        new(player switch
        {
            G.WhichPlayer.You => PlayerKind.You,
            G.WhichPlayer.YourOpponents => PlayerKind.YourOpponents,
            G.WhichPlayer.EachOpponent or G.WhichPlayer.EachOfThatPlayersOpponents => PlayerKind.EachOpponent,
            G.WhichPlayer.EachOtherPlayer => PlayerKind.EachOtherPlayer,
            G.WhichPlayer.EachPlayer => PlayerKind.EachPlayer,
            G.WhichPlayer.OpponentOfYourChoice => PlayerKind.Chosen,
            G.WhichPlayer.AnyOpponent => PlayerKind.AnyOpponent,
            G.WhichPlayer.NoOpponent or G.WhichPlayer.NoPlayer => PlayerKind.NoPlayer,
            G.WhichPlayer.AnyPlayer => PlayerKind.AnyPlayer,
            G.WhichPlayer.Players => PlayerKind.Players,
            G.WhichPlayer.DefendingPlayer => PlayerKind.DefendingPlayer,
            G.WhichPlayer.AttackingPlayer => PlayerKind.AttackingPlayer,
            G.WhichPlayer.EnchantedPlayer => PlayerKind.EnchantedPlayer,
            _ => PlayerKind.AnyPlayer,
        });

    /// <summary>A possessive word as the player it names. "its owner's" and "its controller's" refer to an object the possessive alone can't bind, so they carry an unbound reference.</summary>
    static Selector PlayerOf(G.Whose whose)
    {
        var name = whose.ToString();

        if (name.StartsWith("Your") || name == "EachOfYour")
            return new PlayerSelector(PlayerKind.You);
        if (name is "Opponent")
            return new PlayerSelector(PlayerKind.AnyOpponent);
        if (name is "Opponents" or "EachOpponents")
            return new PlayerSelector(PlayerKind.EachOpponent);
        if (name is "EachPlayers")
            return new PlayerSelector(PlayerKind.EachPlayer);
        if (name.StartsWith("ItsOwner") || name.StartsWith("TheirOwner"))
            return new PlayerSelector(PlayerKind.OwnerOf, new Reference(null, name.StartsWith("Its") ? "its" : "their"));
        if (name.StartsWith("ItsController") || name.StartsWith("TheirController"))
            return new PlayerSelector(PlayerKind.ControllerOf, new Reference(null, name.StartsWith("Its") ? "its" : "their"));
        if (name.StartsWith("Chosen"))
            return new PlayerSelector(PlayerKind.Chosen);
        if (name.StartsWith("Enchanted"))
            return new PlayerSelector(PlayerKind.EnchantedPlayer);
        if (name.StartsWith("ThatPlayer") || name.StartsWith("Their"))
            return new PlayerSelector(PlayerKind.ThatPlayer);

        return Enum.TryParse<PlayerKind>(name, out var kind) ? new PlayerSelector(kind) : new PlayerSelector(PlayerKind.ThatPlayer);
    }

    /// <summary>The names of the vocabulary members among <paramref name="value"/>'s nodes; anything else goes to <paramref name="rest"/>.</summary>
    List<string> Terms(object value, LoweringContext ctx, List<IrNode> rest = null) =>
        TermsOf(value, ctx, rest).Select(x => x.Value).ToList();

    /// <summary>The vocabulary members among <paramref name="value"/>'s nodes, each with its vocabulary; anything else goes to <paramref name="rest"/>.</summary>
    List<Term> TermsOf(object value, LoweringContext ctx, List<IrNode> rest = null)
    {
        List<Term> terms = [];

        foreach (var node in LowerAll(value, ctx))
            if (node is Term term)
                terms.Add(term);
            else if (node is TermSet set)
                terms.AddRange(set.Members);
            else
                rest?.Add(node);

        return terms;
    }

    /// <summary>A kind of object the lowering names itself: "card", "spell", "counter", "any".</summary>
    static Term Kind(string value) => new(value switch
    {
        "card" => nameof(ObjectKind.Card),
        "spell" => nameof(ObjectKind.Spell),
        "any" => nameof(ObjectKind.Any),
        "counter" => nameof(ObjectKind.Counter),
        "token" => nameof(ObjectKind.Token),
        "permanent" => nameof(ObjectKind.Permanent),
        "player" => nameof(ObjectKind.Player),
        "card name" => nameof(ObjectKind.CardName),
        _ => value,
    }, nameof(ObjectKind));

    /// <summary>A comparison from the grammar's words or the symbols the rules use.</summary>
    static Comparison ComparisonOf(string text) =>
        text switch
        {
            "<" or "FewerThan" or "Less" or "Fewer" or "AtMost" => Comparison.Less,
            "<=" or "OrFewer" or "OrLess" => Comparison.LessOrEqual,
            ">" or "MoreThan" or "Greater" or "More" => Comparison.Greater,
            ">=" or "OrMore" or "OrGreater" or "AtLeast" => Comparison.GreaterOrEqual,
            _ => Comparison.Equal,
        };

    /// <summary>A kind of object named by a vocabulary member.</summary>
    /// <summary>A vocabulary member as a term; the bare nouns "card" and "spell" are the IR's own kinds of object.</summary>
    static Term Kind(Enum member) =>
        member switch
        {
            G.CardNoun.Card => new(nameof(ObjectKind.Card), nameof(ObjectKind)),
            G.StackObjectKind.Spell => new(nameof(ObjectKind.Spell), nameof(ObjectKind)),
            _ => new(member.ToString(), member.GetType().Name),
        };

    /// <summary>A qualifier from a vocabulary member, its vocabulary being the member's enum.</summary>
    static Qualifier Q(Enum member, bool negated = false) => new(member.ToString(), negated, member.GetType().Name);

    /// <summary>A qualifier from a word the lowering makes, in a vocabulary it names (see <see cref="Vocabularies"/>).</summary>
    static Qualifier Q(string value, string vocabulary, bool negated = false) => new(value, negated, vocabulary);

    static IReadOnlyList<T> OrNull<T>(List<T> list) => list.Count == 0 ? null : list;

    /// <summary>A narrowing given with the relation it stands in, for <see cref="FilterOf"/>: "from among them" is <c>In(Relation.Among, them)</c>.</summary>
    sealed record In(Relation Relation, object Glyph);

    /// <summary>
    /// An object filter from a glyph's parts: its kinds (vocabulary members), qualifier words, controller phrase,
    /// zone, and any further narrowing, each lowered where a rule exists. Whatever lowers to a qualifier, controller
    /// or zone fills that slot wherever it came from; everything else becomes a <see cref="Predicate"/> (see
    /// <see cref="PredicateOf"/>). A narrowing that is a selector needs its <see cref="Relation"/> said, by wrapping
    /// it in <see cref="In"/>.
    /// </summary>
    ObjectFilter FilterOf(LoweringContext ctx, object kinds = null, object qualifiers = null, object controller = null, object zone = null, params object[] narrowing)
    {
        var filter = new Filter();
        List<IrNode> extra = [];
        filter.Kinds.AddRange(TermsOf(kinds, ctx, extra));

        foreach (var node in extra)
            filter.Add(node, ctx, kinds);

        foreach (var node in LowerAll(qualifiers, ctx))
            if (node is QualifierChoice { Conjunction: Connective.And } all)
                filter.Qualifiers.AddRange(all.Options);
            else
                filter.Add(node, ctx, qualifiers);

        foreach (var node in LowerAll(controller, ctx))
            filter.Add(node, ctx, controller);

        foreach (var node in LowerAll(zone, ctx))
            filter.Add(node, ctx, zone);

        foreach (var part in narrowing)
            if (part is In related)
                filter.With.Add(Lower(related.Glyph, ctx) is Selector to ? new Related(related.Relation, to) : new OpaquePredicate(ctx.TextOf(related.Glyph), related.Glyph.GetType().Name));
            else
                foreach (var node in LowerAll(part, ctx))
                    filter.Add(node, ctx, part);

        return filter.ToFilter();
    }

    /// <summary>An <see cref="ObjectFilter"/> being assembled: lists for the slots, and the sorting of a lowered node into them.</summary>
    sealed class Filter
    {
        public List<Term> Kinds = [];
        public List<Qualifier> Qualifiers = [];
        public Controlled Controller;
        public List<Predicate> With = [];
        public ZoneRef Zone;

        public void Add(IrNode node, LoweringContext ctx, object source)
        {
            switch (node)
            {
                case null:
                    break;
                case Qualifier qualifier:
                    Qualifiers.Add(qualifier);
                    break;
                case Controlled controlled when Controller is null:
                    Controller = controlled;
                    break;
                case ZoneRef zone when Zone is null:
                    Zone = zone;
                    break;
                case ObjectFilter other:
                    Kinds.AddRange(other.Kinds ?? []);
                    Qualifiers.AddRange(other.Qualifiers ?? []);
                    Controller ??= other.Controller;
                    Zone ??= other.Zone;
                    With.AddRange(other.With ?? []);
                    break;
                case Term { Vocabulary: nameof(G.ChosenAspect) } chosen:
                    With.Add(new OfChosen(chosen.Value));
                    break;
                case Term term:
                    Qualifiers.Add(new Qualifier(term.Value, Vocabulary: term.Vocabulary));
                    break;
                default:
                    With.Add(PredicateOf(node, ctx, source));
                    break;
            }
        }

        public ObjectFilter ToFilter() =>
            new(OrNull(Kinds), OrNull(Qualifiers), Controller, OrNull(With), Zone);
    }

    /// <summary>
    /// A lowered node as a predicate: itself when it is one, a condition as <see cref="Satisfying"/>, stats as
    /// <see cref="HasStats"/>, a phase as <see cref="CastDuring"/>; anything else - a selector whose relation wasn't
    /// said, an effect, a value - is opaque, named by the glyph it came from.
    /// </summary>
    static Predicate PredicateOf(IrNode node, LoweringContext ctx, object source) =>
        node switch
        {
            Predicate predicate => predicate,
            Condition condition => new Satisfying(condition),
            StatsMod stats => new HasStats(stats.Power, stats.Toughness),
            DuringPhase during => new CastDuring(during.Phase),
            IOpaque opaque => new OpaquePredicate(opaque.Text, opaque.Glyph),
            _ => new OpaquePredicate(ctx.TextOf(source) ?? node.ToString(), source?.GetType().Name ?? node.GetType().Name),
        };

    /// <summary>Each node as an effect: itself when it is one, else an opaque effect holding its text.</summary>
    static List<Effect> EffectsOf(IEnumerable<IrNode> nodes) =>
        nodes.Select(EffectOf).ToList();

    static Effect EffectOf(IrNode node) =>
        node switch
        {
            Effect effect => effect,
            IOpaque opaque => new OpaqueEffect(opaque.Text, opaque.Glyph),
            _ => new OpaqueEffect(node.ToString(), node.GetType().Name),
        };

    /// <summary>A lowered token: the spec, with or without its count; anything else opaque.</summary>
    static Token TokenOf(IrNode node, LoweringContext ctx, object source) =>
        node switch
        {
            TokenSpecWithCount counted => counted.Token,
            Token token => token,
            IOpaque opaque => new OpaqueToken(opaque.Text, opaque.Glyph),
            _ => new OpaqueToken(ctx.TextOf(source) ?? node?.ToString(), source?.GetType().Name ?? node?.GetType().Name),
        };

    /// <summary>
    /// What a token has beyond its stats: keywords go to <paramref name="keywords"/>, granted abilities (and the
    /// keywords granted alongside them) come back as the token's abilities, and anything else is an opaque ability.
    /// </summary>
    static List<Ability> TokenAbilitiesOf(IEnumerable<IrNode> nodes, List<KeywordInstance> keywords)
    {
        List<Ability> abilities = [];

        foreach (var node in nodes)
            switch (node)
            {
                case null:
                    break;
                case HasKeywords has:
                    keywords.AddRange(has.Keywords.Select(x => new KeywordInstance(x)));
                    break;
                case KeywordInstance keyword:
                    keywords.Add(keyword);
                    break;
                case Pump { Power: null, Toughness: null, BaseStats: null, Duration: null } pump:
                    keywords.AddRange(pump.Keywords ?? []);
                    break;
                case GrantAbility grant:
                    abilities.Add(grant.Ability);
                    keywords.AddRange(grant.Alongside ?? []);
                    break;
                case Sequence sequence:
                    abilities.AddRange(TokenAbilitiesOf(sequence.Steps, keywords));
                    break;
                case Ability ability:
                    abilities.Add(ability);
                    break;
                case IOpaque opaque:
                    abilities.Add(new OpaqueAbility(opaque.Text, opaque.Glyph));
                    break;
                default:
                    abilities.Add(new OpaqueAbility(node.ToString(), node.GetType().Name));
                    break;
            }

        return abilities;
    }

    static ManaOption ManaOptionOf(IrNode node) =>
        node switch
        {
            ManaOption option => option,
            Term { Vocabulary: nameof(G.ManaSymbol) } symbol => new ManaAmount([symbol.Value]),
            IOpaque opaque => new OpaqueManaOption(opaque.Text, opaque.Glyph),
            _ => new OpaqueManaOption(node.ToString(), node.GetType().Name),
        };

    /// <summary>A payment as a <see cref="PayCost"/>: a cost wrapped, an opaque node as an opaque cost.</summary>
    static PayCost PaymentOf(IrNode node, LoweringContext ctx, object source) =>
        node switch
        {
            PayCost pay => pay,
            Cost cost => new PayCost(cost),
            IOpaque opaque => new PayCost(new OpaqueCost(opaque.Text, opaque.Glyph)),
            _ => new PayCost(new OpaqueCost(ctx.TextOf(source) ?? node?.ToString(), source?.GetType().Name ?? node?.GetType().Name)),
        };

    /// <summary>What's done with the cards a search found, after they're found: exiled, or revealed.</summary>
    static Effect SearchFollowUp(string kind, Selector searcher)
    {
        var found = new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [Kind("card")], Qualifiers: [Q(Provenance.Found)]));
        return kind switch
        {
            "Exile" => new Exile(found),
            "Reveal" => new Reveal(searcher, found),
            _ => new OpaqueEffect(kind, nameof(G.SearchFollowUp)),
        };
    }

    /// <summary>A lowered node as an event: itself, a named turn event for a vocabulary member, else opaque.</summary>
    static Event EventOf(IrNode node, LoweringContext ctx, object source) =>
        node switch
        {
            Event happening => happening,
            Term term => new NamedEvent(term.Value),
            IOpaque opaque => new OpaqueEvent(opaque.Text, opaque.Glyph),
            _ => new OpaqueEvent(ctx.TextOf(source) ?? node?.ToString(), source?.GetType().Name ?? node?.GetType().Name),
        };

    /// <summary>
    /// Lowered aspects as terms - a vocabulary member as itself, a kind with qualifiers as a term per part - or null
    /// when one of them is no term at all (a token description, something opaque), which the caller then handles.
    /// </summary>
    static List<Term> AspectsOf(IEnumerable<IrNode> nodes)
    {
        List<Term> terms = [];

        foreach (var node in nodes)
            switch (node)
            {
                case Term term:
                    terms.Add(term);
                    break;
                case QualifiedKind kind:
                    terms.Add(kind.Kind);
                    terms.AddRange(kind.Qualifiers.Select(x => new Term(x.Quality, x.Vocabulary ?? nameof(Qualifier))));
                    break;
                default:
                    return null;
            }

        return terms;
    }

    /// <summary>The subject becoming what the nodes say: terms as a <see cref="BecomesAspect"/>, a token description as an <see cref="Animate"/>, anything else opaque.</summary>
    IrNode BecomesOf(List<IrNode> nodes, bool inAddition, Duration duration, string verb, LoweringContext ctx, object source) =>
        nodes is [TokenSpec spec] ? AnimateOf(spec, ctx.Subject ?? new Self(), inAddition, duration)
        : AspectsOf(nodes) is { } aspects ? new BecomesAspect(ctx.Subject ?? new Self(), aspects, inAddition, Duration: duration, Verb: verb)
        : Opaque(typeof(Effect), ctx.TextOf(source), source.GetType().Name);

    /// <summary>A token description as an animation of the subject: "becomes a 3/3 green elemental creature".</summary>
    static Animate AnimateOf(TokenSpec spec, Selector objects, bool inAddition, Duration duration) =>
        new(objects, spec.Power, spec.Toughness, spec.Colors, spec.Subtypes, spec.Types, null, spec.Keywords, false, null, inAddition, duration);

    static readonly System.Text.RegularExpressions.Regex _baseStats = new(@"^base (power and toughness|power|toughness) ([0-9x]+)(?:/([0-9x]+))?$");

    /// <summary>"base power and toughness 4/4", "base power 8", "base toughness 1" as stats; null when the node isn't that.</summary>
    static StatsMod BaseStatsOf(IrNode node)
    {
        if (node is StatsMod stats)
            return stats;

        if (node is not Term { Vocabulary: nameof(G.BasePowerToughness) } term || _baseStats.Match(term.Value) is not { Success: true } match)
            return null;

        var first = StatOf(match.Groups[2].Value);
        var second = match.Groups[3].Success ? StatOf(match.Groups[3].Value) : null;

        return match.Groups[1].Value switch
        {
            "power" => new StatsMod(first, null),
            "toughness" => new StatsMod(null, first),
            _ => new StatsMod(first, second),
        };
    }

    static Quantity StatOf(string text) =>
        int.TryParse(text, out var value) ? new Fixed(value) : new Variable(text);

    /// <summary>Counters a lowered node describes: a spec as itself, or the counters a put-counters effect places.</summary>
    static CounterSpec CountersOf(IrNode node) =>
        node switch
        {
            CounterSpec spec => spec,
            PutCounters put => put.Counters,
            _ => null,
        };

    /// <summary>A selector narrowed by a further predicate, or by a qualifier word, when it has a filter to take it.</summary>
    static Selector Narrow(Selector selector, IrNode narrowing, LoweringContext ctx, object source = null)
    {
        if (narrowing is null || selector is not ObjectSelector objects)
            return selector;

        var filter = objects.Filter ?? new ObjectFilter();
        return narrowing is Qualifier qualifier
            ? objects with { Filter = filter with { Qualifiers = [.. filter.Qualifiers ?? [], qualifier] } }
            : objects with { Filter = filter with { With = [.. filter.With ?? [], PredicateOf(narrowing, ctx, source)] } };
    }

    static Selector Narrow(Selector selector, Relation relation, Selector to) =>
        selector is ObjectSelector objects
            ? objects with { Filter = (objects.Filter ?? new ObjectFilter()) with { With = [.. objects.Filter?.With ?? [], new Related(relation, to)] } }
            : selector;

    /// <summary>The keywords among <paramref name="value"/>'s nodes, as instances; anything else goes to <paramref name="other"/>.</summary>
    List<KeywordInstance> KeywordsOf(object value, LoweringContext ctx, List<IrNode> other)
    {
        List<KeywordInstance> keywords = [];

        foreach (var node in LowerAll(value, ctx))
            switch (node)
            {
                case KeywordInstance keyword:
                    keywords.Add(keyword);
                    break;
                case KeywordLine line:
                    keywords.AddRange(line.Keywords);
                    other.AddRange(line.Other ?? []);
                    break;
                case Term { Vocabulary: nameof(G.Keyword) } term:
                    keywords.Add(new KeywordInstance(term.Value));
                    break;
                default:
                    other.Add(node);
                    break;
            }

        return keywords;
    }

    static string SymbolText(G.ManaSymbol symbol) =>
        symbol.Colorless is int amount ? $"{{{amount}}}"
        : symbol.Symbol is { } kind ? "{" + PatternOf(kind) + "}"
        : "{?}";

    /// <summary>A vocabulary member as the text that matches it: its first pattern, else its name in lower case.</summary>
    static string PatternOf(Enum member) =>
        member.GetType().GetField(member.ToString())?.GetCustomAttribute<RegexPatternAttribute>()?.Patterns.FirstOrDefault()?.Replace("\\", "")
        ?? member.ToString().ToLowerInvariant();

    PhaseRef PhaseRefOf(object value, LoweringContext ctx) =>
        Lower(value, ctx) switch
        {
            PhaseRef phase => phase,
            UntilPhase until => until.Phase,
            DuringPhase during => during.Phase,
            _ => null,
        };

    Event PhaseEventOf(object value, LoweringContext ctx) =>
        PhaseRefOf(value, ctx) is { } phase ? new PhaseEvent(phase) : (Event)Opaque(typeof(Event), ctx.TextOf(value), value?.GetType().Name);

    ZoneRef ZoneOf(object value, LoweringContext ctx) =>
        Lower(value, ctx) switch
        {
            ZoneRef zone => zone,
            Term term => new ZoneRef(term.Value),
            _ => null,
        };

    static Duration DurationOf(IrNode node) =>
        node switch
        {
            null => null,
            Duration duration => duration,
            Condition condition => new While(condition),
            PhaseRef phase => new UntilPhase(phase),
            IOpaque opaque => new OpaqueDuration(opaque.Text, opaque.Glyph),
            _ => new OpaqueDuration(node.ToString(), node.GetType().Name),
        };
}
