namespace MTGCardParser.TokenUnits;

public class CardKeyword : TokenUnit
{
    public CardKeyword() : base(nameof(Keyword)) { }

    public Keyword Keyword { get; set; }
}