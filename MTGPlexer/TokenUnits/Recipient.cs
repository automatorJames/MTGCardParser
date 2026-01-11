namespace MTGPlexer.TokenUnits;

[Dependent]
public class Recipient : TokenUnitOneOf
{
    public TargetableEntity TargetableEntity { get; set; }
    public ThatCardsController ThatCardsController { get; set; }
}
