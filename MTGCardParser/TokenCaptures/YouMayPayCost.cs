namespace MTGCardParser.TokenCaptures;

[NoSpaces]
public class YouMayPayCost : ITokenUnit
{
    public RegexTemplate<YouMayPayCost> RegexTemplate => new("you may pay ", new CaptureAlternatives(nameof(ManaValue), nameof(LifeQuantity)), @"\.");

    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}

