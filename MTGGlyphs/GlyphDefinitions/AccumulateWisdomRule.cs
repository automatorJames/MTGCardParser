namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Accumulate Wisdom, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Accumulate Wisdom</exampledoc>
[TokenizationOrder(-1)]
public class AccumulateWisdomRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put each of those cards into your hand instead if there are three or more lesson cards in your graveyard"];
}
