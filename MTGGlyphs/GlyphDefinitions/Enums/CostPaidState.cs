namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a stated alternative mana cost was paid, as a condition: "the {2}{u} cost was paid", "that cost wasn't paid".</summary>
public enum CostPaidState
{
    [RegexPattern("was paid")]
    WasPaid,

    [RegexPattern("wasn't paid")]
    WasNotPaid
}
