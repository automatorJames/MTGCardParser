namespace MTGPlexer.TokenUnits;

public class OptionalPayCost : TokenUnit
{
    public override Snippet[] Snippets => [Prop(PayOptionType), " pay ", Prop(Cost)];

    public PayOptionType PayOptionType { get; set; }
    public Cost Cost { get; set; }
}

public enum PayOptionType
{
    UnlessYou,
    YouMay
}