namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wanderwine Farewell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wanderwine Farewell</exampledoc>
[TokenizationOrder(-1)]
public class WanderwineFarewellRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if you control a merfolk, create a 1/1 white and blue merfolk creature token for each permanent returned to its owner's hand this way"];
}
