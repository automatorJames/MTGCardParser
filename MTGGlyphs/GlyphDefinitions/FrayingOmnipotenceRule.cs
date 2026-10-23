namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fraying Omnipotence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fraying Omnipotence</exampledoc>
[TokenizationOrder(-1)]
public class FrayingOmnipotenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player loses half their life, then discards half the cards in their hand, then sacrifices half the creatures they control of their choice"];
}
