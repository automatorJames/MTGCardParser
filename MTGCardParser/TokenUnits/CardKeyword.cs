namespace MTGCardParser.TokenUnits;

public class CardKeyword : TokenUnitBase
{
    public RegexTemplate<CardKeyword> RegexTemplate => new(nameof(Keyword));

    public Keyword? Keyword { get; set; }
}