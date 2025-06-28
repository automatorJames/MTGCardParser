namespace MTGCardParser.TokenCaptures;

public class Punctuation : TokenCaptureBase<Punctuation>
{
    public override RegexTemplate<Punctuation> RegexTemplate => new(nameof(PunctuationCharacter));

    public PunctuationCharacter? PunctuationCharacter { get; set; }
}

[RegexOptions(WrapInWordBoundaries = false, OptionalPlural = false)]
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