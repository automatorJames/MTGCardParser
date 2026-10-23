namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Unleash the Inferno, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unleash the Inferno</exampledoc>
/// <examplecapture>destroy target artifact or enchantment an opponent controls with mana value less than or equal to that amount of excess damage</examplecapture>
[Dependent]
public class UnleashTheInfernoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target artifact or enchantment an opponent controls with mana value less than or equal to that amount of excess damage"];
}
