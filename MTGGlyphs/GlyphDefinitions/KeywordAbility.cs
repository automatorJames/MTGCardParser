namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One keyword ability: a plain keyword ("flying"), one with a parameter ("protection from red", "rampage 2", "cycling {2}", "bushido 1", "affinity for artifacts", "gift a card", "champion a goblin", "suspend 3—{w}", "kicker—sacrifice a land", "prototype {1}{w}{w} — 3/3", "snow swampwalk", "craft with artifact {2}{u}"), or cumulative upkeep with its cost.</summary>
/// <exampledoc>Repentant Blacksmith</exampledoc>
/// <examplecapture>protection from red</examplecapture>
[Dependent]
public class KeywordAbility : GlyphOneOf
{
    public CraftWith Craft { get; set; }
    public Protection Protection { get; set; }
    public CumulativeUpkeep CumulativeUpkeep { get; set; }
    public Rampage Rampage { get; set; }
    public AffinityFor Affinity { get; set; }
    public GiftKeyword Gift { get; set; }
    public ChampionKeyword Champion { get; set; }
    public KeywordWithAmountAndCost WithAmountAndCost { get; set; }
    public KeywordWithActionCost WithActionCost { get; set; }
    public KeywordWithCostAndStats WithCostAndStats { get; set; }
    public KeywordWithCost WithCost { get; set; }
    public KeywordWithAmount WithAmount { get; set; }
    public QualifiedLandwalk QualifiedLandwalk { get; set; }
    public Keyword? Keyword { get; set; }
}
