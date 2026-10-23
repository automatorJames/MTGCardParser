namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nicanzil, Current Conductor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicanzil, Current Conductor</exampledoc>
[Dependent]
public class NicanzilCurrentConductorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a land card from your hand onto the battlefield tapped"];
}
