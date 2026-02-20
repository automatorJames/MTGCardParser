namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class PowerToughnessMod : TokenUnit
{
    public override Snippet[] Snippets => [Prop(PowerSign), Prop(PowerValue), "/", Prop(ToughnessSign), Prop(ToughnessValue)];

    public PlusMinus PowerSign { get; set; }
    public Quantity PowerValue { get; set; }
    public PlusMinus ToughnessSign { get; set; }
    public Quantity ToughnessValue { get; set; }
}