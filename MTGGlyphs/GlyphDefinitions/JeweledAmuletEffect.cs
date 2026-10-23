namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jeweled Amulet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jeweled Amulet</exampledoc>
[Dependent]
public class JeweledAmuletEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add one mana of this artifact's last noted type"];
}
