namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wall of Shards, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Shards</exampledoc>
[TokenizationOrder(-1)]
public class WallOfShardsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—an opponent gains 1 life"];
}
