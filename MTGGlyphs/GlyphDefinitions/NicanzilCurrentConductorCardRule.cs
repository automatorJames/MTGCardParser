namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nicanzil, Current Conductor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicanzil, Current Conductor</exampledoc>
[TokenizationOrder(-1)]
public class NicanzilCurrentConductorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control explores a land card, you may put a land card from your hand onto the battlefield tapped"];
}
