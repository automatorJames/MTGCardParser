namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class OptionalPayCost : TokenUnit
{
    protected override string[] Snippets => [nameof(PayOptionType), " pay ", nameof(Cost)];

    public PayOptionType PayOptionType { get; set; }
    public Cost Cost { get; set; }
}

public enum PayOptionType
{
    UnlessYou,
    YouMay
}