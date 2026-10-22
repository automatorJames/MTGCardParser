namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A "with …" qualifier on what's targeted: keywords ("with flying"), a bounded characteristic ("with power 2 or less"), an exact one ("with mana value x"), one compared with this creature's ("with greater power", "with power less than this creature's power"), one bounded by a count ("with power less than or equal to the number of creatures you control"), one equal to a referenced spell's ("with that spell's mana value"), the most extreme one ("with the greatest power among creatures you control"), counters on it ("with a +1/+1 counter on it"), "without summoning sickness", a combat relation ("blocking or blocked by this creature", "it's blocking"), no two sharing one ("with different powers"), a name shared with something named earlier ("with the same name as that creature"), this card's name ("named {this}", "not named {this}"), or a basic land type ("with a basic land type"). A glyph wrapping a one-of, so it can be optional.</summary>
/// <exampledoc>Dwarven Warriors</exampledoc>
/// <examplecapture>with power 2 or less</examplecapture>
[Dependent]
public class WithQualifier : Glyph
{
    public override Nib[] Nibs => [Prop(Condition)];

    public OneOf<OneOf<WithKeywords, WithDifferent, OneOf<WithSameName, NamedThis, WithBasicLandType>>, OneOf<WithCharacteristicBound, WithCharacteristicValue, OneOf<WithCharacteristicBoundByCount, WithObjectsCharacteristic>>, OneOf<WithRelativeCharacteristic, WithCountersOnIt, OneOf<WithoutSummoningSickness, OneOf<CombatRelation, InCombatWithIt>, OneOf<WithCharacteristicComparedTo, WithExtremeCharacteristic>>>> Condition { get; set; }
}
