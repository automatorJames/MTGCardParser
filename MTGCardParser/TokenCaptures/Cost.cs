namespace MTGCardParser.TokenCaptures;

public class Cost : ITokenUnit
{
    public RegexTemplate<Cost> RegexTemplate => new("you may pay ", nameof(ManaValue), @"\.");

    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}


