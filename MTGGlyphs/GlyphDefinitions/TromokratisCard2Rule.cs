namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tromokratis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tromokratis</exampledoc>
[TokenizationOrder(-1)]
public class TromokratisCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "can't be blocked unless all creatures defending player controls block it"];
}
