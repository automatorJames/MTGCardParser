namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glen Elendra's Answer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glen Elendra's Answer</exampledoc>
[TokenizationOrder(-1)]
public class GlenElendrasAnswerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 blue and black faerie creature token with flying for each spell and ability countered this way"];
}
