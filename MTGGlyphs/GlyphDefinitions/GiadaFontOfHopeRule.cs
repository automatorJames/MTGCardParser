namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Giada, Font of Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giada, Font of Hope</exampledoc>
[TokenizationOrder(-1)]
public class GiadaFontOfHopeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other angel you control enters with an additional +1/+1 counter on it for each angel you already control"];
}
