namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a player's life went up or down, in life-change conditions and amounts: "gained", "lost", "have gained" ("you gained 4 or more life", "the amount of life you lost this turn", "your opponents have lost 2 or more life").</summary>
public enum LifeChange
{
    [RegexPattern("gained", "have gained")]
    Gained,

    [RegexPattern("lost", "have lost")]
    Lost
}
