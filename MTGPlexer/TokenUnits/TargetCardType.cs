namespace MTGPlexer.TokenUnits;

public class TagetCardType : TokenUnit
{
    public TagetCardType() : base("target", nameof(CardType)) { }

    public CardType CardType { get; set; }
}