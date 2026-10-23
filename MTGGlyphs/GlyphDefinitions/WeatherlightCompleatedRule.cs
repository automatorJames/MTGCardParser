namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Weatherlight Compleated, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Weatherlight Compleated</exampledoc>
[TokenizationOrder(-1)]
public class WeatherlightCompleatedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it doesn't, scry 1"];
}
