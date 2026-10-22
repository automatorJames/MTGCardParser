namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A price to pay: mana ("{u}"), tapping ("{t}", or another permanent: "tap enchanted creature"), life ("1 life", "pay 1 life", "life equal to its toughness"), mana and life together ("{1} and 1 life"), an object's mana cost ("its mana cost"), sacrificing ("sacrifice {this}"), exiling ("exile {this}"), discarding ("discard a card at random", "discard this card", "discards their hand"), milling ("mill two cards"), removing counters ("remove a corpse counter from {this}"), returning a permanent ("return a land you control to its owner's hand"), or a cumulative upkeep's own cost ("its upkeep cost").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
[Dependent]
public class Cost : GlyphOneOf
{
    public ManaAndLifeCost ManaAndLife { get; set; }
    public ManaCost ManaCost { get; set; }
    public TapSymbol? TapSymbol { get; set; }
    public TapOrUntap TapPermanent { get; set; }
    public LifeEqualTo LifeEqualTo { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
    public ObjectManaCost ObjectManaCost { get; set; }
    public Sacrifice Sacrifice { get; set; }
    public DestroyTarget Exile { get; set; }
    public DiscardThisCard DiscardThis { get; set; }
    public DiscardHand DiscardHand { get; set; }
    public DrawOrDiscardCards Discard { get; set; }
    public PlaceCounters RemoveCounters { get; set; }
    public ReturnCard Return { get; set; }
    public ItsUpkeepCost UpkeepCost { get; set; }
    public PayLifeOrEnergy PayLifeOrEnergy { get; set; }
    public Mill Mill { get; set; }
}
