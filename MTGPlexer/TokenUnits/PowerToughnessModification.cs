namespace MTGPlexer.TokenUnits;

[NoSpaces]
[NoWordBoundary]
public class PowerToughnessModification : TokenUnit
{
    public PowerToughnessModification() : base(nameof(PowerSign), nameof(PowerValue), "/", nameof(ToughnessSign), nameof(ToughnessValue)) { }

    public PlusMinus PowerSign { get; set; }
    public Quantity PowerValue { get; set; }
    public PlusMinus ToughnessSign { get; set; }
    public Quantity ToughnessValue { get; set; }
}

