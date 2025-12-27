namespace MTGPlexer.TokenUnits;

[Dependent]
public class AnyTarget : TokenUnitOneOf
{
    public PlayerIdentity PlayerIdentity { get; set; }
    public CardType CardType { get; set; }
    public CreatureType CreatureType { get; set; }
}