namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A "with …" qualifier on what's targeted: keywords ("with flying"), a keyword named as an ability ("with a cycling ability"), a bounded characteristic ("with power 2 or less"), an exact one ("with mana value x"), one compared with this creature's ("with greater power", "with power less than this creature's power"), one bounded by a count ("with power less than or equal to the number of creatures you control"), one equal to a referenced spell's ("with that spell's mana value"), the most extreme one ("with the greatest power among creatures you control"), counters on it ("with a +1/+1 counter on it"), "without summoning sickness", a combat relation ("blocking or blocked by this creature", "it's blocking"), no two sharing one ("with different powers"), a name shared with something named earlier ("with the same name as that creature") or chosen earlier ("with the chosen name"), this card's name ("named {this}", "not named {this}"), a basic land type ("with a basic land type"), or the same controller as an earlier target ("with the same controller"). The alternatives are tried in this order.</summary>
/// <exampledoc>Dwarven Warriors</exampledoc>
/// <examplecapture>with power 2 or less</examplecapture>
[Dependent]
public class WithQualifier : GlyphOneOf
{
    public WithKeywords Keywords { get; set; }
    public WithAbilityNamed AbilityNamed { get; set; }
    public WithDifferent Different { get; set; }
    public WithSameName SameName { get; set; }
    public WithChosenName ChosenName { get; set; }
    public NamedThis NamedThis { get; set; }
    public WithBasicLandType BasicLandType { get; set; }
    public WithSameController SameController { get; set; }
    public WithCharacteristicBound CharacteristicBound { get; set; }
    public WithCharacteristicValue CharacteristicValue { get; set; }
    public WithCharacteristicBoundByCount CharacteristicBoundByCount { get; set; }
    public WithObjectsCharacteristic ObjectsCharacteristic { get; set; }
    public WithRelativeCharacteristic RelativeCharacteristic { get; set; }
    public WithCountersOnIt CountersOnIt { get; set; }
    public WithoutSummoningSickness WithoutSummoningSickness { get; set; }
    public CombatRelation CombatRelation { get; set; }
    public InCombatWithIt InCombatWithIt { get; set; }
    public WithCharacteristicComparedTo CharacteristicComparedTo { get; set; }
    public WithExtremeCharacteristic ExtremeCharacteristic { get; set; }
}
