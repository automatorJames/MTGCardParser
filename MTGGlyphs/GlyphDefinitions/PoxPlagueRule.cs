namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pox Plague, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pox Plague</exampledoc>
[TokenizationOrder(-1)]
public class PoxPlagueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player loses half their life, then discards half the cards in their hand, then sacrifices half the permanents they control of their choice"];
}
