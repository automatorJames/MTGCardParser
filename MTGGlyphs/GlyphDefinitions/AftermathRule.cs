namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Aftermath's reminder text.</summary>
/// <exampledoc>Appeal // Authority</exampledoc>
/// <examplecapture>cast this spell only from your graveyard</examplecapture>
public class AftermathRule : Glyph
{
    public override Nib[] Nibs => ["cast this spell only from your graveyard"];
}
