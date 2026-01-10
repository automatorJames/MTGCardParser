namespace MTGPlexer.TokenUnits;

[NoSpaces]
[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
public class PowerToughnessMod : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(PowerSign), Prop(PowerValue), "/", Prop(ToughnessSign), Prop(ToughnessValue)];

    public PlusMinus PowerSign { get; set; }
    public Quantity PowerValue { get; set; }
    public PlusMinus ToughnessSign { get; set; }
    public Quantity ToughnessValue { get; set; }
}