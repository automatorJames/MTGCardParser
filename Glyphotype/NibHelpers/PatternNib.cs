namespace Glyphotype.NibHelpers;

/// <summary>
/// A nib whose text is a regex rather than literal text - the explicit opt-in (see <see cref="GlyphPrimitives.Glyph.Pattern"/>).
/// Its text is used exactly as written, as one unit (a top-level <c>|</c> in it never splits the glyph around it). A
/// pattern that can match nothing - <c>(an?)?</c> - is optional, and spaced as any optional part is.
/// </summary>
public record PatternNib : Nib
{
    public PatternNib(string pattern)
        : base(pattern, pattern)
    {
        IsOptional = MatchesEmpty(pattern);
    }

    public override bool IsOptional { get; }

    public override string Authored => $"Pattern({Quote(Text)})";

    /// <summary>A pattern isn't literal text, so whether it binds to its neighbours can't be read off it.</summary>
    public override IReadOnlyList<string> Literals => null;

    static bool MatchesEmpty(string pattern)
    {
        try
        {
            return System.Text.RegularExpressions.Regex.IsMatch("", $"^(?:{pattern})$");
        }
        catch (ArgumentException)
        {
            // An invalid pattern fails when the glyph's regex is compiled, with a better message than this could give.
            return false;
        }
    }
}
