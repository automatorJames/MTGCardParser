namespace Glyphotype.NibHelpers;

/// <summary>
/// One piece of a Glyph's pattern, as listed in <see cref="GlyphPrimitives.Glyph.Nibs"/>. A string nib is literal
/// text - matched exactly as written, with any regex metacharacter in it (<c>$</c>, <c>{</c>, <c>?</c>, ...) taken
/// literally - so authoring a nib never requires knowing regex. Regex is an explicit opt-in:
/// <see cref="PatternNib"/> (see <see cref="GlyphPrimitives.Glyph.Pattern"/>).
/// </summary>
public record Nib
{
    /// <summary>The nib as authored: the literal text, or for a <see cref="PatternNib"/> its regex.</summary>
    public string Text { get; init; }

    /// <summary>The regex this nib contributes: <see cref="Text"/> escaped for a literal nib, <see cref="Text"/> itself for a <see cref="PatternNib"/>.</summary>
    public string Regex { get; init; }

    /// <summary>Whether this nib can match nothing at all - an <see cref="OptionalNib"/>, or a <see cref="PatternNib"/> whose regex matches the empty string - so it's spaced as any optional part is (see <see cref="JoinerRules.PlaceLeadingJoiner"/>).</summary>
    public virtual bool IsOptional => false;

    /// <summary>
    /// The literal texts this nib is made of, as authored - the one text of a plain nib, each of a helper's
    /// (<see cref="GlyphPrimitives.Glyph.Alt"/>, <see cref="GlyphPrimitives.Glyph.Opt"/>, ...) - or null when it isn't
    /// literal text (a <see cref="PatternNib"/>, a <see cref="PropertyNib"/>). Any of them may end the nib's match,
    /// so they're what decides whether the nib binds to what follows it (see <see cref="JoinSite.BeforeClosings"/>).
    /// </summary>
    public virtual IReadOnlyList<string> Literals => [Text];

    /// <summary>The nib as it was written in a <see cref="GlyphPrimitives.Glyph.Nibs"/> list, e.g. <c>"the"</c> or <c>Alt("a", "b")</c> - for messages.</summary>
    public virtual string Authored => Quote(Text);

    /// <summary><paramref name="texts"/> as a helper call's arguments: <c>"a", "b"</c>.</summary>
    protected static string Quote(params IEnumerable<string> texts) =>
        string.Join(", ", texts.Select(x => "\"" + x.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""));

    /// <summary>A literal-text nib.</summary>
    public Nib(string text)
        : this(text, EscapeLiteral(text))
    {
    }

    protected Nib(string text, string regex)
    {
        Text = text;
        Regex = regex;
    }

    /// <summary>Refuses a helper's texts unless there are at least <paramref name="minimum"/>, none of them empty - the helpers take literal text, and only literal text (see <see cref="GlyphPrimitives.Glyph.Alt"/>).</summary>
    protected static string[] RequireTexts(string helper, string[] texts, int minimum)
    {
        if (texts is null || texts.Length < minimum || texts.Any(string.IsNullOrEmpty))
            throw new ArgumentException(minimum > 1
                ? $"{helper}() takes {minimum} or more non-empty texts - a single text is just the literal itself"
                : $"{helper}() takes one or more non-empty texts", nameof(texts));

        return texts;
    }

    /// <summary>
    /// The ways this nib's match can open - one regex per alternative, which together match what <see cref="Regex"/>
    /// does - so the separator in front of it can be decided for each as for a nib of its own (see
    /// <see cref="JoinerRules.ForOpenings"/>). <paramref name="joiner"/> is the joiner of the group the nib sits in,
    /// for a nib whose parts are separated as nibs are (see <see cref="SomeNib"/>). Just <see cref="Regex"/> by default.
    /// </summary>
    public virtual IReadOnlyList<string> Branches(Joiner joiner) => [Regex];

    /// <summary>A plain string in a Nibs list is literal text.</summary>
    public static implicit operator Nib(string str) => new(str);

    /// <summary>The characters that are special in a .NET regex outside a character class - the set <see cref="System.Text.RegularExpressions.Regex.Escape"/> escapes, less whitespace and <c>#</c> (which only matter under <see cref="System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace"/>, and spaces are escaped separately - see <see cref="BuiltRegex.EscapeSpaces"/>).</summary>
    static readonly HashSet<char> _metacharacters = ['\\', '*', '+', '?', '|', '{', '[', '(', ')', '^', '$', '.'];

    /// <summary>The regex matching <paramref name="text"/> exactly.</summary>
    public static string EscapeLiteral(string text) =>
        text is null ? null : string.Concat(text.Select(c => _metacharacters.Contains(c) ? "\\" + c : c.ToString()));
}
