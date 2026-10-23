namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sanar, Innovative First-Year, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanar, Innovative First-Year</exampledoc>
[TokenizationOrder(-1)]
public class SanarInnovativeFirstYearRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each of those colors, you may exile a card of that color from among the revealed cards"];
}
