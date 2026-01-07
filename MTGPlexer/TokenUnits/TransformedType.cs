namespace MTGPlexer.TokenUnits;

[Dependent]
public class TransformedType : TokenUnit
{
    protected override string[] Snippets => ["an?", nameof(CardTypeCompound)];

    public CompoundOf<CardType> CardTypeCompound { get; set; }
}