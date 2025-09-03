namespace MTGPlexer.TokenUnits;

public class Buff : TokenUnitOneOf
{
    public PowerToughnessMod PowerToughnessModification { get; set; }
    public CardKeyword CardKeyword { get; set; }
}

 