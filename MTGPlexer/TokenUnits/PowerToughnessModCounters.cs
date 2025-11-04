namespace MTGPlexer.TokenUnits;

public class PowerToughnessModCounters : TokenUnit
{
    protected override string[] Snippets => [nameof(Quantity), nameof(PowerToughnessMod), "counter(s)?"];

    public Quantity Quantity { get; set; }
    public PowerToughnessMod PowerToughnessMod { get; set; }
}