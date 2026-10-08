namespace Glyphotype.NibHelpers;

/// <summary>
/// A glyph matched in place, with no property to hold it: it gets a capture group, a capture and a value of its own,
/// but nothing is assigned to the glyph whose nib it is - there's nothing it would add. Authored only as
/// <see cref="Nib.This"/>; the capture is named for <see cref="GlyphType"/> ("This"), numbered where a glyph embeds it
/// more than once ("This1", "This2").
/// </summary>
public sealed record EmbeddedGlyphNib : Nib
{
    /// <summary>The glyph matched.</summary>
    public Type GlyphType { get; }

    internal EmbeddedGlyphNib(Type glyphType)
        : base(glyphType.Name, null)
    {
        GlyphType = glyphType;
    }
}
