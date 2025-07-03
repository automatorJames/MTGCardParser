namespace MTGCardParser.TokenUnits;

[IgnoreInAnalysis]
public class Punctuation : ITokenUnit
{
    public RegexTemplate<Punctuation> RegexTemplate => new(nameof(PunctuationCharacter));

    public PunctuationCharacter? PunctuationCharacter { get; set; }
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