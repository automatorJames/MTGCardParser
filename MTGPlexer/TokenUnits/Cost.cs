namespace MTGPlexer.TokenUnits;

public class Cost : TokenUnitOneOf
{
    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
    public PunctuationTerminal PunctuationTerminal { get; set; }
}

