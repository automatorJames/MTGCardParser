namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
[NoWordBoundary]
[Color("#999999")]
public class PunctuationTerminal : TokenUnit
{
    public PunctuationCharacter PunctuationCharacter { get; set; }
}

[RegexEnum]
public enum PunctuationCharacter
{
    [RegexPattern(@"\.")] 
    Period,

    [RegexPattern(@",")]
    Comma,

    [RegexPattern(@";")]
    Semicolon
}