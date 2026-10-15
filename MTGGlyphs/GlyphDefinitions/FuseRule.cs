namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Fuse's reminder text.</summary>
/// <exampledoc>Wear // Tear</exampledoc>
/// <examplecapture>you may cast one or both halves of this card from your hand</examplecapture>
public class FuseRule : Glyph
{
    public override Nib[] Nibs => ["you may cast one or both halves of this card from your hand"];
}
