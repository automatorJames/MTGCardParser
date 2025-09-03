namespace MTGPlexer.TokenUnits;

public class GainedOrLostBuff : TokenUnitOneOf
//public class GainedOrLostBuff : TokenUnit
{
    public PowerToughnessModification PowerToughnessModification { get; set; }
    public CardKeyword CardKeyword { get; set; }
}

 