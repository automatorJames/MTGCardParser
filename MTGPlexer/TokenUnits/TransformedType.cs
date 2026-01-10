namespace MTGPlexer.TokenUnits;

[Dependent]
public class TransformedType : TokenUnit
{
    protected override Snippet[] Snippets => ["an?", Prop(CardType)];

    public CompoundOf<CardType> CardType { get; set; }
}