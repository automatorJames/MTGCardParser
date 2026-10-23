namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Luxurious Locomotive, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luxurious Locomotive</exampledoc>
[Dependent]
public class LuxuriousLocomotiveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a treasure token for each creature that crewed it this turn"];
}
