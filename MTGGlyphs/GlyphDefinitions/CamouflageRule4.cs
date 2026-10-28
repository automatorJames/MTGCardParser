namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Camouflage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Camouflage</exampledoc>
[TokenizationOrder(-1)]
public class CamouflageRule4 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature in a pile that can block the creature that pile is assigned to does so"];
}
