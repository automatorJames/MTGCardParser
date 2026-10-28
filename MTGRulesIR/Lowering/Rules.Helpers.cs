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
    List<string> Terms(object value, LoweringContext ctx, List<IrNode> rest = null)
    {
        List<string> names = [];

        foreach (var node in LowerAll(value, ctx))
            if (node is Term term)
                names.Add(term.Value);
            else if (node is TermSet set)
                names.AddRange(set.Values);
            else
                rest?.Add(node);

        return names;
    }

    static IReadOnlyList<T> OrNull<T>(List<T> list) => list.Count == 0 ? null : list;

    /// <summary>
    /// An object filter from a glyph's parts: its kinds (vocabulary members), qualifier words, controller phrase,
    /// zone, and any further narrowing (<paramref name="with"/>), each lowered where a rule exists and kept as a
    /// node under <see cref="ObjectFilter.With"/> otherwise.
    /// </summary>
    ObjectFilter FilterOf(LoweringContext ctx, object kinds = null, object qualifiers = null, object controller = null, object zone = null, params object[] narrowing)
    {
        List<IrNode> extra = [];
        var kindNames = Terms(kinds, ctx, extra);

        List<Qualifier> qualifierList = [];
        foreach (var node in LowerAll(qualifiers, ctx))
            if (node is Qualifier qualifier)
                qualifierList.Add(qualifier);
            else if (node is Term term)
                qualifierList.Add(new Qualifier(term.Value));
            else
                extra.Add(node);

        Controlled controlled = null;
        foreach (var node in LowerAll(controller, ctx))
            if (node is Controlled c)
                controlled = c;
            else
                extra.Add(node);

        ZoneRef zoneRef = null;
        foreach (var node in LowerAll(zone, ctx))
            if (node is ZoneRef z)
                zoneRef = z;
            else if (node is Term term)
                zoneRef = new ZoneRef(term.Value);
            else
                extra.Add(node);

        foreach (var part in narrowing)
            extra.AddRange(LowerAll(part, ctx));

        return new ObjectFilter(OrNull(kindNames), OrNull(qualifierList), controlled, OrNull(extra), zoneRef);
    }

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
