namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flunk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flunk</exampledoc>
[TokenizationOrder(-1)]
public class FlunkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature gets -x/-x until end of turn, where x is 7 minus the number of cards in that creature's controller's hand"];
}
