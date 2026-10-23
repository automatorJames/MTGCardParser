namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Telling Time, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Telling Time</exampledoc>
[TokenizationOrder(-1)]
public class TellingTimeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put one of those cards into your hand, one on top of your library, and one on the bottom of your library"];
}
