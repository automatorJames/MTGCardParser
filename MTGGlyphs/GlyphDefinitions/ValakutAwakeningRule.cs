namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Valakut Awakening // Valakut Stoneforge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Valakut Awakening // Valakut Stoneforge</exampledoc>
[TokenizationOrder(-1)]
public class ValakutAwakeningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put any number of cards from your hand on the bottom of your library, then draw that many cards plus one"];
}
