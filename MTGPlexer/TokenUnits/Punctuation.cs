namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
[Color("#999999")]
public class Punctuation : TokenUnit
{
    public PunctuationCharacter PunctuationCharacter { get; set; }
}

[EnumOptions(WrapInWordBoundaries = false, OptionalPlural = false)]
public enum PunctuationCharacter
{
    [RegexPattern(@"\.")] 
    Period,

    [RegexPattern(@",")]
    Comma,

    [RegexPattern(@"""")]
    Quote,

    [RegexPattern(@";")]
    Semicolon
}