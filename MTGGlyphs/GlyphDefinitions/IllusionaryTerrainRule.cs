namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illusionary Terrain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illusionary Terrain</exampledoc>
[TokenizationOrder(-1)]
public class IllusionaryTerrainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["basic lands of the first chosen type are the second chosen type"];
}
