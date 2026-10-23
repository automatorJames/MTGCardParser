namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unleash the Inferno, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unleash the Inferno</exampledoc>
[TokenizationOrder(-1)]
public class UnleashTheInfernoCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when it deals excess damage this way, destroy target artifact or enchantment an opponent controls with mana value less than or equal to that amount of excess damage"];
}
