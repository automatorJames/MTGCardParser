namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Oath of Kaya, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oath of Kaya</exampledoc>
[TokenizationOrder(-1)]
public class OathOfKayaCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent attacks a planeswalker you control with one or more creatures,", Nib.This, "deals 2 damage to that player and you gain 2 life"];
}
