namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Second Guess, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Second Guess</exampledoc>
[TokenizationOrder(-1)]
public class SecondGuessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell that's the second spell cast this turn"];
}
