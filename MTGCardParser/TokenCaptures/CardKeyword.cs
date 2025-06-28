namespace MTGCardParser.TokenCaptures;

public class CardKeyword : ITokenCapture
{
    public RegexTemplate<CardKeyword> RegexTemplate => new(nameof(Keyword));

    public Keyword? Keyword { get; set; }
}