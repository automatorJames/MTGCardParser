namespace MTGPlexer.TokenUnits;

[Dependent]
public class TransformedType : TokenUnit
{
    protected override string[] Snippets => ["an?", nameof(CardType)];

    public CompoundOf<CardType> CardType { get; set; }
}