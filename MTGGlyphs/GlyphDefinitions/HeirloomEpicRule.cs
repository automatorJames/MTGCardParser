namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Heirloom Epic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heirloom Epic</exampledoc>
[TokenizationOrder(-1)]
public class HeirloomEpicRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each mana in this ability's activation cost, you may tap an untapped creature you control rather than pay that mana"];
}
