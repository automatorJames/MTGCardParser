namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spider-Punk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Punk</exampledoc>
[TokenizationOrder(-1)]
public class SpiderPunkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells and abilities can't be countered"];
}
