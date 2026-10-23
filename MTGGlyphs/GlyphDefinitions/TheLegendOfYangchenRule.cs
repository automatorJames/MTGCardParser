namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Legend of Yangchen // Avatar Yangchen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Legend of Yangchen // Avatar Yangchen</exampledoc>
[TokenizationOrder(-1)]
public class TheLegendOfYangchenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["i — starting with you, each player chooses up to one permanent with mana value 3 or greater from among permanents your opponents control"];
}
