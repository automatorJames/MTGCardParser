namespace MTGPlexer.TokenUnits;

[Dependent]
public class CardOrCreatureTypeTarget : TokenUnitOneOf
{
    public CardType CardType { get; set; }
    public CreatureType CreatureType { get; set; }
}