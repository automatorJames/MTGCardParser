namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Inferno of the Star Mounts, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inferno of the Star Mounts</exampledoc>
[Dependent]
public class InfernoOfTheStarMountsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 20 damage to any target"];
}
