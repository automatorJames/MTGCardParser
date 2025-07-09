using MTGCardParser.BaseClasses;

namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class YouMayPayCost : TokenUnit
{
    public RegexTemplate<YouMayPayCost> RegexTemplate => new("you may pay ", new AlternativeTokenUnits(nameof(ManaValue), nameof(LifeQuantity)), @"\.");

    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}

