namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wheel and Deal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wheel and Deal</exampledoc>
[TokenizationOrder(-1)]
public class WheelAndDealRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any number of target opponents each discard their hands, then draw seven cards"];
}
