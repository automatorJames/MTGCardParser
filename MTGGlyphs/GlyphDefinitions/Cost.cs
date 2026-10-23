namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A price to pay: mana ("{u}"), tapping ("{t}", or another permanent: "tap enchanted creature"), life ("1 life", "pay 1 life", "life equal to its toughness"), mana and life together ("{1} and 1 life"), an object's mana cost ("its mana cost"), sacrificing ("sacrifice {this}"), exiling ("exile {this}", "exile the top two cards of your library", "exile the top creature card of your graveyard"), discarding ("discard a card at random", "discard this card", "discards their hand", "discard another card named {this}"), milling ("mill two cards"), removing counters ("remove a corpse counter from {this}"), removing counters and then sacrificing or returning the permanent ("remove three quest counters from this enchantment and sacrifice it"), returning a permanent ("return a land you control to its owner's hand"), putting cards somewhere ("put a card from your graveyard on the bottom of your library"), beholding ("behold a dragon"), revealing cards ("reveal this card from your hand", "reveal a blue card in your hand"), exerting ("exert this creature"), unattaching an equipment ("unattach {this}", "unattach this equipment"), a cumulative upkeep's own cost ("its upkeep cost"), a numbered keyword action ("collect evidence 3", "blight 1"), or a keyword action with a mana payment ("waterbend {5}", "forage").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
[Dependent]
public class Cost : GlyphOneOf
{
    public CountersAndSacrificeCost CountersAndSacrifice { get; set; }
    public ManaAndLifeCost ManaAndLife { get; set; }
    public ManaCost ManaCost { get; set; }
    public TapSymbol? TapSymbol { get; set; }
    public TapOrUntap TapPermanent { get; set; }
    public LifeEqualTo LifeEqualTo { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
    public ObjectManaCost ObjectManaCost { get; set; }
    public Sacrifice Sacrifice { get; set; }
    public DestroyTarget Exile { get; set; }
    public ExileTopOfLibrary ExileTop { get; set; }
    public DiscardThisCard DiscardThis { get; set; }
    public DiscardNamedCard DiscardNamed { get; set; }
    public DiscardHand DiscardHand { get; set; }
    public DrawOrDiscardCards Discard { get; set; }
    public PlaceCounters RemoveCounters { get; set; }
    public ReturnCard Return { get; set; }
    public PutIntoZone PutCards { get; set; }
    public Behold Behold { get; set; }
    public Exert Exert { get; set; }
    public Unattach Unattach { get; set; }
    public ItsUpkeepCost UpkeepCost { get; set; }
    public PayLifeOrEnergy PayLifeOrEnergy { get; set; }
    public Mill Mill { get; set; }
    public Reveal Reveal { get; set; }
    public KeywordWithAmount KeywordAction { get; set; }
    public KeywordActionCost KeywordActionWithPayment { get; set; }
}
