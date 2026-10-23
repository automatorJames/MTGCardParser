namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Back for Seconds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Back for Seconds</exampledoc>
/// <examplecapture>if this spell was bargained, you may put one of those cards with mana value 4 or less onto the battlefield instead of putting it into your hand</examplecapture>
[TokenizationOrder(-1)]
public class BackForSecondsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was bargained, you may put one of those cards with mana value 4 or less onto the battlefield instead of putting it into your hand"];
}
