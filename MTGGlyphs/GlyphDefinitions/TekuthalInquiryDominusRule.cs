namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tekuthal, Inquiry Dominus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tekuthal, Inquiry Dominus</exampledoc>
[TokenizationOrder(-1)]
public class TekuthalInquiryDominusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}{u/p}{u/p}, remove three counters from among other artifacts, creatures, and planeswalkers you control: put an indestructible counter on tekuthal"];
}
