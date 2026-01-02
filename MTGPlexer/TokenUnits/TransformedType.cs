namespace MTGPlexer.TokenUnits;

[Dependent]
public class TransformedType : TokenUnit
{
    protected override string[] Snippets => ["an?", nameof(CardType)];

    public CardType CardType { get; set; }
}