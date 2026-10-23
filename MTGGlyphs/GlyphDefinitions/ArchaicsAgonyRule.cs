namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Archaic's Agony, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Archaic's Agony</exampledoc>
[TokenizationOrder(-1)]
public class ArchaicsAgonyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile cards from the top of your library equal to the excess damage dealt to that creature this way"];
}
