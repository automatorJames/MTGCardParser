namespace MTGCardParser.TokenUnits;

public class CardKeyword : TokenUnit
{
    public RegexTemplate<CardKeyword> RegexTemplate => new(nameof(Keyword));

    public Keyword? Keyword { get; set; }
}