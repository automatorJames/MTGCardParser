namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yare</exampledoc>
[TokenizationOrder(-1)]
public class YareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can block up to two additional creatures this turn"];
}
