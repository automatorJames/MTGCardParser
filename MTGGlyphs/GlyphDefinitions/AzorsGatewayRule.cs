namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Azor's Gateway // Sanctum of the Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Azor's Gateway // Sanctum of the Sun</exampledoc>
[TokenizationOrder(-1)]
public class AzorsGatewayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if cards with five or more different mana values are exiled with azor's gateway, you gain 5 life, untap azor's gateway, and transform it"];
}
