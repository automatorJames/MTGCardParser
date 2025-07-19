namespace MTGPlexer.TokenUnits;

[EnclosesToken]
[IgnoreInAnalysis]
[Color("#999999")]
public class PunctuationEnclosing : TokenUnit
{
    public EnclosingPunctuationCharacter EnclosingPunctuationCharacter { get; set; }
}

[RegexEnum(WrapInWordBoundaries = false, OptionalPlural = false)]
public enum EnclosingPunctuationCharacter
{
    [RegexPattern(@"""")]
    DoubleQuote,
}