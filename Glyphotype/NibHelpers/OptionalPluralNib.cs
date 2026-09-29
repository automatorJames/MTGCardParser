namespace Glyphotype.NibHelpers;

/// <summary>An optional plural suffix on the word before it (see <see cref="GlyphPrimitives.Glyph.Plural"/>).</summary>
public record OptionalPluralNib : Nib
{
    const string _suffixes = "(s|es|ies)?";

    public OptionalPluralNib()
        : base(_suffixes, _suffixes) { }
}
