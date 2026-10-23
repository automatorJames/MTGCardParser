namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Haunting Wind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haunting Wind</exampledoc>
/// <examplecapture>this enchantment deals 1 damage to that artifact's controller</examplecapture>
[Dependent]
public class HauntingWindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals 1 damage to that artifact's controller"];
}
