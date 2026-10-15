namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A price to pay: mana ("{u}"), tapping ("{t}", or another permanent: "tap enchanted creature"), life ("1 life"), sacrificing ("sacrifice {this}"), exiling ("exile {this}"), discarding ("discard a card at random", "discard this card"), removing counters ("remove a corpse counter from {this}"), or a cumulative upkeep's own cost ("its upkeep cost").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
[Dependent]
public class Cost : GlyphOneOf
{
    public ManaCost ManaCost { get; set; }
    public TapSymbol? TapSymbol { get; set; }
    public TapOrUntap TapPermanent { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
    public Sacrifice Sacrifice { get; set; }
    public DestroyTarget Exile { get; set; }
    public DiscardThisCard DiscardThis { get; set; }
    public DrawOrDiscardCards Discard { get; set; }
    public PlaceCounters RemoveCounters { get; set; }
    public ItsUpkeepCost UpkeepCost { get; set; }
}
