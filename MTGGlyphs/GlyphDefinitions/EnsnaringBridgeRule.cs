namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ensnaring Bridge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ensnaring Bridge</exampledoc>
[TokenizationOrder(-1)]
public class EnsnaringBridgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures with power greater than the number of cards in your hand can't attack"];
}
