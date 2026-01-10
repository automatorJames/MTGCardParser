namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class OptionalPayCost : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(PayOptionType), " pay ", Prop(Cost)];

    public PayOptionType PayOptionType { get; set; }
    public Cost Cost { get; set; }
}

public enum PayOptionType
{
    UnlessYou,
    YouMay
}