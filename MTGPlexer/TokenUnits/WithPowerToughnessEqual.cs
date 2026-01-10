namespace MTGPlexer.TokenUnits;

public class WithPowerToughnessEqual : TokenUnit
{
    protected override string[] Snippets => ["with", nameof(PowerAndOrToughness), "(each )?", "equal to", nameof(EquivalentToMeasurement)];

    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public EquivalentToMeasurement EquivalentToMeasurement { get; set; }
}
