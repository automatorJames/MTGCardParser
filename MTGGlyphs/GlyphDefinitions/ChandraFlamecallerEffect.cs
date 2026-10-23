namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chandra, Flamecaller, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra, Flamecaller</exampledoc>
[Dependent]
public class ChandraFlamecallerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard all the cards in your hand, then draw that many cards plus one"];
}
