namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reverse Polarity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reverse Polarity</exampledoc>
[TokenizationOrder(-1)]
public class ReversePolarityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain x life, where x is twice the damage dealt to you so far this turn by artifacts"];
}
