namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Retrace's reminder text.</summary>
/// <exampledoc>Call the Skybreaker</exampledoc>
/// <examplecapture>you may cast this card from your graveyard by discarding a land card in addition to paying its other costs</examplecapture>
public class RetraceRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard by discarding a land card in addition to paying its other costs"];
}
