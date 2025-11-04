namespace MTGPlexer.TokenUnits;

public class TagetCardType : TokenUnit
{
    protected override string[] Snippets => ["target", nameof(CardType)];

    public CardType CardType { get; set; }
}