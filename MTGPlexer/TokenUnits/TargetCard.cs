namespace MTGPlexer.TokenUnits;

public class TargetCard : TokenUnit
{
    protected override string[] Snippets => ["target", nameof(CardType)];

    public CardType CardType { get; set; }
}