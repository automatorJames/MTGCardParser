namespace MTGCardParser.TokenCaptures;

public class CardKeyword : ITokenCapture
{
    public string RegexTemplate => $@"§{nameof(Keyword)}§";

    public Keyword? Keyword { get; set; }
}