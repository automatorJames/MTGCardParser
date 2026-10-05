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

    public bool IsOptional { get; init; }

    /// <summary>A literal-text nib.</summary>
    public Nib(string text)
        : this(text, EscapeLiteral(text))
    {
    }

    protected Nib(string text, string regex)
    {
        Text = text;
        Regex = regex;
        IsOptional = this is OptionalNib;
    }

    /// <summary>A plain string in a Nibs list is literal text.</summary>
    public static implicit operator Nib(string str) => new(str);

    /// <summary>The characters that are special in a .NET regex outside a character class - the set <see cref="System.Text.RegularExpressions.Regex.Escape"/> escapes, less whitespace and <c>#</c> (which only matter under <see cref="System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace"/>, and spaces are escaped separately - see <see cref="BuiltRegex.EscapeSpaces"/>).</summary>
    static readonly HashSet<char> _metacharacters = ['\\', '*', '+', '?', '|', '{', '[', '(', ')', '^', '$', '.'];

    /// <summary>The regex matching <paramref name="text"/> exactly.</summary>
    public static string EscapeLiteral(string text) =>
        text is null ? null : string.Concat(text.Select(c => _metacharacters.Contains(c) ? "\\" + c : c.ToString()));
}
