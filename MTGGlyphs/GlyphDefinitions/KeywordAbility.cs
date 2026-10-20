namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One keyword ability: a plain keyword ("flying"), one with a parameter ("protection from red", "rampage 2", "cycling {2}", "bushido 1", "affinity for artifacts", "gift a card"), or cumulative upkeep with its cost.</summary>
/// <exampledoc>Repentant Blacksmith</exampledoc>
/// <examplecapture>protection from red</examplecapture>
[Dependent]
public class KeywordAbility : GlyphOneOf
{
    public Protection Protection { get; set; }
    public CumulativeUpkeep CumulativeUpkeep { get; set; }
    public Rampage Rampage { get; set; }
    public AffinityFor Affinity { get; set; }
    public GiftKeyword Gift { get; set; }
    public KeywordWithCost WithCost { get; set; }
    public KeywordWithAmount WithAmount { get; set; }
    public Keyword? Keyword { get; set; }
}
