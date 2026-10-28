namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Inferno of the Star Mounts, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inferno of the Star Mounts</exampledoc>
[TokenizationOrder(-1)]
public class InfernoOfTheStarMountsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when its power becomes 20 this way, it deals 20 damage to any target"];
}
