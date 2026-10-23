namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jeweled Bird, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jeweled Bird</exampledoc>
[TokenizationOrder(-1)]
public class JeweledBirdRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, put all other cards you own from the ante into your graveyard, then draw a card"];
}
