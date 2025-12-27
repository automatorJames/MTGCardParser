namespace MTGPlexer.TokenUnits;

public class TagetCard : TokenUnit
{
    protected override string[] Snippets => ["target", nameof(CardType)];

    public CardType CardType { get; set; }
}