namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fiery Bombardment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Bombardment</exampledoc>
/// <examplecapture>this enchantment deals damage to any target equal to the number of red mana symbols in the sacrificed creature's mana cost</examplecapture>
[Dependent]
public class FieryBombardmentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals damage to any target equal to the number of red mana symbols in the sacrificed creature's mana cost"];
}
