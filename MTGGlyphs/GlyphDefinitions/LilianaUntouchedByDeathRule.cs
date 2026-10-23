namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Liliana, Untouched by Death, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana, Untouched by Death</exampledoc>
[TokenizationOrder(-1)]
public class LilianaUntouchedByDeathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if at least one zombie card is milled this way, each opponent loses 2 life and you gain 2 life"];
}
