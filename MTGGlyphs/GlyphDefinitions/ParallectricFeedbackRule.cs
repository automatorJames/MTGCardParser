namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Parallectric Feedback, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parallectric Feedback</exampledoc>
[TokenizationOrder(-1)]
public class ParallectricFeedbackRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to target spell's controller equal to that spell's mana value"];
}
