namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Multiple Choice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Multiple Choice</exampledoc>
[TokenizationOrder(-1)]
public class MultipleChoiceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 4 or more, do all of the above"];
}
