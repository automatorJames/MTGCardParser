namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Umbra Mystic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Umbra Mystic</exampledoc>
[TokenizationOrder(-1)]
public class UmbraMysticCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras attached to permanents you control have umbra armor"];
}
