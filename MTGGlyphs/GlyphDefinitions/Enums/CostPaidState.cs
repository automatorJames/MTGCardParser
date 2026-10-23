namespace MTGGlyphs.GlyphDefinitions;

public enum CostPaidState
{
    [RegexPattern("was paid")]
    WasPaid,

    [RegexPattern("wasn't paid")]
    WasNotPaid
}
