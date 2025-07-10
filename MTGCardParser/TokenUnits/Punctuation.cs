namespace MTGCardParser.TokenUnits;

[IgnoreInAnalysis]
public class Punctuation : TokenUnit
{
    public RegexTemplate RegexTemplate => new(nameof(PunctuationCharacter));

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