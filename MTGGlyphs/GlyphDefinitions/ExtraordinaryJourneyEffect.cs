namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Extraordinary Journey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Extraordinary Journey</exampledoc>
/// <examplecapture>if one or more of them entered from exile or was cast from exile, you draw a card</examplecapture>
[Dependent]
public class ExtraordinaryJourneyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if one or more of them entered from exile or was cast from exile, you draw a card"];
}
