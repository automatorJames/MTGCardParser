namespace MTGPlexer.TokenUnits;

public class PowerToughnessModCounters : TokenUnit
{
    public PowerToughnessModCounters() : base(nameof(Quantity), nameof(PowerToughnessMod), "counter(s)?") { }
    public Quantity Quantity { get; set; }
    public PowerToughnessMod PowerToughnessMod { get; set; }

}

