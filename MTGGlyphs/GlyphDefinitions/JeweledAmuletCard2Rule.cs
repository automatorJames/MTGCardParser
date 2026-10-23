namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jeweled Amulet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jeweled Amulet</exampledoc>
[TokenizationOrder(-1)]
public class JeweledAmuletCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["note the type of mana spent to pay this activation cost"];
}
