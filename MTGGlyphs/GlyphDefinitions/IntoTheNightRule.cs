namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Into the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Into the Night</exampledoc>
[TokenizationOrder(-1)]
public class IntoTheNightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard any number of cards, then draw that many cards plus one"];
}
