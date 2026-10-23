namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Debt of Loyalty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Debt of Loyalty</exampledoc>
[TokenizationOrder(-1)]
public class DebtOfLoyaltyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain control of that creature if it regenerates this way"];
}
