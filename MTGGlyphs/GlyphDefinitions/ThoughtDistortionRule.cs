namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thought Distortion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought Distortion</exampledoc>
[TokenizationOrder(-1)]
public class ThoughtDistortionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all noncreature, nonland cards from that player's hand and graveyard"];
}
