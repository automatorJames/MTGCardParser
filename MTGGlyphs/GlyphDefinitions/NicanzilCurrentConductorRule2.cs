namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nicanzil, Current Conductor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicanzil, Current Conductor</exampledoc>
[TokenizationOrder(-1)]
public class NicanzilCurrentConductorRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control explores a nonland card, put a +1/+1 counter on nicanzil"];
}
