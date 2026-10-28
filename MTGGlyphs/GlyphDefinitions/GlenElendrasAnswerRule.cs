namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glen Elendra's Answer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glen Elendra's Answer</exampledoc>
[TokenizationOrder(-1)]
public class GlenElendrasAnswerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter all spells your opponents control and all abilities your opponents control"];
}
