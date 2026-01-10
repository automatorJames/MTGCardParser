namespace MTGPlexer.TokenUnits;

public class WithPowerToughnessEqual : TokenUnit
{
    protected override Snippet[] Snippets => ["with", Prop(PowerAndOrToughness), new OptionalSnippet("each"), "equal to", Prop(EquivalentToMeasurement)];

    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public EquivalentToMeasurement EquivalentToMeasurement { get; set; }
}
