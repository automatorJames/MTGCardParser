namespace Glyphotype.NibHelpers;

/// <summary>A nib whose text is a regex rather than literal text - the explicit opt-in (see <see cref="GlyphPrimitives.Glyph.Pattern"/>). Its text is used exactly as written.</summary>
public record PatternNib : Nib
{
    public PatternNib(string pattern)
        : base(pattern, pattern) { }
}
