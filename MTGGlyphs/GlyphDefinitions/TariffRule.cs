namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tariff, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tariff</exampledoc>
[TokenizationOrder(-1)]
public class TariffRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player sacrifices the creature they control with the greatest mana value unless they pay that creature's mana cost"];
}
