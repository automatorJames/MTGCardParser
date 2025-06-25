namespace MTGCardParser.TokenCaptures;

[RegOpt(DoNotWrapInWordBoundaries = true)]
public class Punctuation : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(PunctuationCharacter)}§";
    public PunctuationCharacter? PunctuationCharacter { get; set; }
}

public enum PunctuationCharacter
{
    [RegPat(@"\.")] 
    Period,

    [RegPat(@",")]
    Comma,

    [RegPat(@"""")]
    Quote,

    [RegPat(@";")]
    Semicolon
}