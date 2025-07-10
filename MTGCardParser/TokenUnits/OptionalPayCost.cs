namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class OptionalPayCost : TokenUnit
{
    public RegexTemplate RegexTemplate => new(nameof(PayOptionType), " pay ", new AlternativeTokenUnits(nameof(ManaValue), nameof(LifeQuantity)), @"\.");

    public PayOptionType PayOptionType { get; set; }
    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}

public enum PayOptionType
{
    UnlessYou,
    YouMay
}

