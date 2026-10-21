namespace MTGGlyphs.GlyphDefinitions;

public enum TypeCopula
{
    [RegexPattern("is an?")]
    IsA,

    Are,

    [RegexPattern("becomes? an?")]
    BecomesA,

    Is
}
