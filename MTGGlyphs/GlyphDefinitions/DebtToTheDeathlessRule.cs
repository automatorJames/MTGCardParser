namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Debt to the Deathless, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Debt to the Deathless</exampledoc>
[TokenizationOrder(-1)]
public class DebtToTheDeathlessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent loses two times x life"];
}
