namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shipwreck Sifters, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shipwreck Sifters</exampledoc>
[TokenizationOrder(-1)]
public class ShipwreckSiftersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you discard a spirit card or a card with disturb, put a +1/+1 counter on this creature"];
}
