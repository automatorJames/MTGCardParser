namespace MTGPlexer.TokenUnits;

public class WithPowerToughnessEqual : TokenUnit
{
    protected override Snippet[] Snippets => ["with", Prop(PowerAndOrToughness), Opt("each"), "equal to", Prop(EquivalentToMeasurement)];

    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public EquivalentToMeasurement EquivalentToMeasurement { get; set; }
}
