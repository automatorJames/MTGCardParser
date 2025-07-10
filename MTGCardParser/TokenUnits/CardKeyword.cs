namespace MTGCardParser.TokenUnits;

public class CardKeyword : TokenUnit
{
    public RegexTemplate RegexTemplate => new(nameof(Keyword));

    public Keyword Keyword { get; set; }
}