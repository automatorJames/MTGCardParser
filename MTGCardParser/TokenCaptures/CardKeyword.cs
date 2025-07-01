namespace MTGCardParser.TokenCaptures;

public class CardKeyword : ITokenUnit
{
    public RegexTemplate<CardKeyword> RegexTemplate => new(nameof(Keyword));

    public Keyword? Keyword { get; set; }
}