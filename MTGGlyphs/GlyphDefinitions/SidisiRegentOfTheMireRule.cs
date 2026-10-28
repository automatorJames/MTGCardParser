namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sidisi, Regent of the Mire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sidisi, Regent of the Mire</exampledoc>
[TokenizationOrder(-1)]
public class SidisiRegentOfTheMireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, sacrifice a creature you control with mana value x other than sidisi: return target creature card with mana value x plus 1 from your graveyard to the battlefield"];
}
