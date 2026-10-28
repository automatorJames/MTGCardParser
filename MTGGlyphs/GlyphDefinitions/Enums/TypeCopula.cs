namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb of a type-setting static effect: "is a" or "is an" ("is an island"), "are" ("are artifacts in addition to their other types"), "becomes a" or "becomes an", or bare "is" ("is the chosen type in addition to its other types").</summary>
public enum TypeCopula
{
    [RegexPattern("is an?")]
    IsA,

    Are,

    [RegexPattern("becomes? an?")]
    BecomesA,

    Is
}
