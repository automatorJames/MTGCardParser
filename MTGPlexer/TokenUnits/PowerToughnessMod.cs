namespace MTGPlexer.TokenUnits;

[NoSpaces]
[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
public class PowerToughnessMod : TokenUnit
{
    protected override string[] Snippets => [nameof(PowerSign), nameof(PowerValue), "/", nameof(ToughnessSign), nameof(ToughnessValue)];

    public PlusMinus PowerSign { get; set; }
    public Quantity PowerValue { get; set; }
    public PlusMinus ToughnessSign { get; set; }
    public Quantity ToughnessValue { get; set; }
}