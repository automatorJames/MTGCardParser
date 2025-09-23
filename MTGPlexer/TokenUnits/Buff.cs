namespace MTGPlexer.TokenUnits;

[TokenUnitProperty]
public class Buff : TokenUnitOneOf
{
    public PowerToughnessMod PowerToughnessModification { get; set; }
    //public CardKeyword CardKeyword { get; set; }
    public Keyword Keyword { get; set; }
}

 