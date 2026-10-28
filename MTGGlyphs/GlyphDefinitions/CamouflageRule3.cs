namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Camouflage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Camouflage</exampledoc>
[TokenizationOrder(-1)]
public class CamouflageRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["assign each pile to a different one of those attacking creatures at random"];
}
