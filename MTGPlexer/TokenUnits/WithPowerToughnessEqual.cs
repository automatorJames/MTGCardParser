namespace MTGPlexer.TokenUnits;

public class WithPowerToughnessEqual : TokenUnit
{
    public override Snippet[] Snippets => ["with", Prop(PowerAndOrToughness), Opt("each"), "equal to", Prop(EquivalentToMeasurement)];

    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public EquivalentToMeasurement EquivalentToMeasurement { get; set; }
}
