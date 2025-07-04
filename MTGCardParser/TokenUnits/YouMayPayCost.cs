namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class YouMayPayCost : TokenUnitBase
{
    public RegexTemplate<YouMayPayCost> RegexTemplate => new("you may pay ", new CaptureAlternatives(nameof(ManaValue), nameof(LifeQuantity)), @"\.");

    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}

