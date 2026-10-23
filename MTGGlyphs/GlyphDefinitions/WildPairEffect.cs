namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wild Pair, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wild Pair</exampledoc>
/// <examplecapture>if you cast it from your hand, you may search your library for a creature card with the same total power and toughness, put it onto the battlefield, then shuffle</examplecapture>
[Dependent]
public class WildPairEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast it from your hand, you may search your library for a creature card with the same total power and toughness, put it onto the battlefield, then shuffle"];
}
