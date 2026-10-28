namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose keyword costs a cost change applies to: "flashback costs you pay", "flashback costs your opponents pay".</summary>
public enum CostPayer
{
    [RegexPattern("you pay")]
    You,

    [RegexPattern("your opponents pay")]
    Opponents
}
