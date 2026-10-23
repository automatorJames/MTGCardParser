namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cut Short, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cut Short</exampledoc>
[TokenizationOrder(-1)]
public class CutShortRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target planeswalker that was activated this turn or tapped creature"];
}
