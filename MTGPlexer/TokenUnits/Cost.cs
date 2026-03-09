namespace MTGPlexer.TokenUnits;

public class Cost : TokenUnitOneOf
{
    public ManaValueItem ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}

 