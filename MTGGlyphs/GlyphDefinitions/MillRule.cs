namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mill's reminder text, for one card.</summary>
/// <exampledoc>Deranged Assistant</exampledoc>
/// <examplecapture>to mill a card, put the top card of your library into your graveyard</examplecapture>
public class MillRule : Glyph
{
    public override Nib[] Nibs => ["to mill a card, put the top card of your library into your graveyard"];
}
