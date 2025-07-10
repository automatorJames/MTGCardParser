namespace MTGCardParser.TokenUnits;

public class Cost : TokenUnit
{
    public Cost() : base("you may pay ", nameof(ManaValue), @"\.") { }

    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}


