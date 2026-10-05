namespace Glyphotype.NibHelpers;

/// <summary>
/// At least one of several literal texts, in the order written (see <see cref="GlyphPrimitives.Glyph.Some"/>):
/// any non-empty run of them, each present or absent, but never none. Equivalent to an <see cref="NibAlternatives"/>
/// of every such run - <c>Some("a", "b", "c")</c> is <c>a</c>, <c>b</c>, <c>c</c>, <c>a b</c>, <c>a c</c>,
/// <c>b c</c> or <c>a b c</c> - without spelling out all 2ⁿ-1 of them.
/// </summary>
public record SomeNib : Nib
{
    /// <summary>The items as originally passed to <see cref="Glyphotype.GlyphPrimitives.Glyph.Some"/>, so a display-only consumer can reconstruct the original <c>Some(...)</c> call.</summary>
    public string[] Items { get; }

    public SomeNib(params string[] items)
        : base("(" + string.Join('|', RequireTexts("Some", items, minimum: 2)) + ")+", BuildRegex(items, Joiner.Space))
    {
        Items = items;
    }

    public override string Authored => $"Some({Quote(Items)})";

    public override IReadOnlyList<string> Literals => Items;

    /// <summary>
    /// One branch per item that could be the first present - that item, then each later one optionally - with
    /// the items separated by <paramref name="joiner"/> under the same rules as separate nibs (see
    /// <see cref="JoinerRules.Between"/>): so <c>Some(",", " ")</c> renders <c>,</c>, <c> </c> or <c>, </c>, and
    /// <c>Some("big", "red")</c> renders <c>big</c>, <c>red</c> or <c>big red</c>.
    /// </summary>
    public override IReadOnlyList<string> Branches(Joiner joiner) =>
        BuildBranches(Items, joiner);

    static string BuildRegex(string[] items, Joiner joiner) =>
        BuildBranches(items, joiner) is { Count: > 0 } branches ? "(" + string.Join('|', branches) + ")" : null;

    static IReadOnlyList<string> BuildBranches(string[] items, Joiner joiner)
    {
        if (items is null || items.Length == 0 || items.Any(string.IsNullOrEmpty))
            return [];

        // A pipe only ever separates alternatives, never the parts of a sequence (see JoinerRules.Intended).
        if (joiner == Joiner.Pipe)
            joiner = Joiner.None;

        var escaped = items.Select(EscapeLiteral).ToArray();

        // Each item is separated from the one written before it (the nearest that may precede it).
        var separated = escaped
            .Select((x, i) => JoinerRules.Between(new JoinSite(joiner, AfterText: BuiltRegex.EscapeSpaces(x), BeforeClosings: i > 0 ? [items[i - 1]] : null)) is var separator && separator != Joiner.None
                ? separator.GetDescription() + x
                : x)
            .ToArray();

        return Enumerable.Range(0, items.Length)
            .Select(first => escaped[first] + string.Concat(separated.Skip(first + 1).Select(x => $"({x})?")))
            .ToList();
    }
}
