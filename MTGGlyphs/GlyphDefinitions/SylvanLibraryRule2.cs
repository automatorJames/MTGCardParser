namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sylvan Library, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sylvan Library</exampledoc>
[TokenizationOrder(-1)]
public class SylvanLibraryRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each of those cards, pay 4 life or put the card on top of your library"];
}
