namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Paradoxical Outcome, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Paradoxical Outcome</exampledoc>
[TokenizationOrder(-1)]
public class ParadoxicalOutcomeRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each card returned to your hand this way"];
}
