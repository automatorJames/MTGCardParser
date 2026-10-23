namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Living Terrain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Terrain</exampledoc>
[TokenizationOrder(-1)]
public class LivingTerrainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted land is a 5/6 green treefolk creature that's still a land"];
}
