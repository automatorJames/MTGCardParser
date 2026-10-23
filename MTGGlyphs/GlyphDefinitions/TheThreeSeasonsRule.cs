namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Three Seasons, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Three Seasons</exampledoc>
[TokenizationOrder(-1)]
public class TheThreeSeasonsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["their owners shuffle those cards into their libraries"];
}
