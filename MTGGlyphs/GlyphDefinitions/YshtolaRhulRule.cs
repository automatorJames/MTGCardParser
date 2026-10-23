namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Y'shtola Rhul, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Y'shtola Rhul</exampledoc>
[TokenizationOrder(-1)]
public class YshtolaRhulRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if it's the first end step of the turn, there is an additional end step after this step"];
}
