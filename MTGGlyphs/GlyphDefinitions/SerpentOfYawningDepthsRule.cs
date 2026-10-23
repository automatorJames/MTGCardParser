namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serpent of Yawning Depths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serpent of Yawning Depths</exampledoc>
[TokenizationOrder(-1)]
public class SerpentOfYawningDepthsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["krakens, leviathans, octopuses, and serpents you control can't be blocked except by krakens, leviathans, octopuses, and serpents"];
}
