namespace MTGPlexer.TokenUnits;

public class PowerToughnessModCounters : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(Quantity), Prop(PowerToughnessMod), "counter(s)?"];

    public Quantity Quantity { get; set; }
    public PowerToughnessMod PowerToughnessMod { get; set; }
}