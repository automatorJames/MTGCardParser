namespace MTGPlexer.TokenUnits;

[Dependent]
public class CardAspect() : TokenUnitOneOf
{
    public CardType CardType { get; set; }
    public ManaColor ManaColor { get; set; }
}
