namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Rebound's reminder text.</summary>
/// <exampledoc>Distortion Strike</exampledoc>
/// <examplecapture>if you cast this spell from your hand, exile it as it resolves</examplecapture>
public class ReboundRule : Glyph
{
    public override Nib[] Nibs => ["if you cast this spell from your hand, exile it as it resolves"];
}
