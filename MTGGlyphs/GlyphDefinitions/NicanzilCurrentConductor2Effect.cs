namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nicanzil, Current Conductor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicanzil, Current Conductor</exampledoc>
[Dependent]
public class NicanzilCurrentConductor2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on nicanzil"];
}
