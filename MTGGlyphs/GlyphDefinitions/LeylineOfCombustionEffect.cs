namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Leyline of Combustion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leyline of Combustion</exampledoc>
/// <examplecapture>this enchantment deals 2 damage to that player</examplecapture>
[Dependent]
public class LeylineOfCombustionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals 2 damage to that player"];
}
