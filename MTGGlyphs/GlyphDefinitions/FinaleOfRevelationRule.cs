namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Finale of Revelation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Finale of Revelation</exampledoc>
/// <examplecapture>if x is 10 or more, instead shuffle your graveyard into your library, draw x cards, untap up to five lands, and you have no maximum hand size for the rest of the game</examplecapture>
[TokenizationOrder(-1)]
public class FinaleOfRevelationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 10 or more, instead shuffle your graveyard into your library, draw x cards, untap up to five lands, and you have no maximum hand size for the rest of the game"];
}
