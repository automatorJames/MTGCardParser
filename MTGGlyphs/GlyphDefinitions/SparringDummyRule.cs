namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sparring Dummy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sparring Dummy</exampledoc>
[TokenizationOrder(-1)]
public class SparringDummyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 2 life if a lesson card is milled this way"];
}
