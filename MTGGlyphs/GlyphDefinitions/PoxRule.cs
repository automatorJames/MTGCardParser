namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pox</exampledoc>
/// <examplecapture>each player loses a third of their life, then discards a third of the cards in their hand, then sacrifices a third of the creatures they control of their choice, then sacrifices a third of the lands they control of their choice</examplecapture>
[TokenizationOrder(-1)]
public class PoxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player loses a third of their life, then discards a third of the cards in their hand, then sacrifices a third of the creatures they control of their choice, then sacrifices a third of the lands they control of their choice"];
}
