namespace MTGPlexer.TokenUnits;

[FollowsToken]
[IgnoreInAnalysis]
[Color("#999999")]
public class PunctuationTerminal : TokenUnit
{
    public PunctuationCharacter PunctuationCharacter { get; set; }
}

[RegexEnum(WrapInWordBoundaries = false, OptionalPlural = false)]
public enum PunctuationCharacter
{
    [RegexPattern(@"\.")] 
    Period,

    [RegexPattern(@",")]
    Comma,

    [RegexPattern(@";")]
    Semicolon
}