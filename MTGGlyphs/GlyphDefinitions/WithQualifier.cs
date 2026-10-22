namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A "with …" qualifier on what's targeted: keywords ("with flying"), a bounded characteristic ("with power 2 or less"), an exact one ("with mana value x"), one compared with this creature's ("with greater power", "with power less than this creature's power"), counters on it ("with a +1/+1 counter on it"), "without summoning sickness", a combat relation ("blocking or blocked by this creature"), no two sharing one ("with different powers"), or a name shared with something named earlier ("with the same name as that creature"). A glyph wrapping a one-of, so it can be optional.</summary>
/// <exampledoc>Dwarven Warriors</exampledoc>
/// <examplecapture>with power 2 or less</examplecapture>
[Dependent]
public class WithQualifier : Glyph
{
    public override Nib[] Nibs => [Prop(Condition)];

    public OneOf<OneOf<WithKeywords, WithDifferent, WithSameName>, OneOf<WithCharacteristicBound, WithCharacteristicValue>, OneOf<WithRelativeCharacteristic, WithCountersOnIt, OneOf<WithoutSummoningSickness, CombatRelation, WithCharacteristicComparedTo>>> Condition { get; set; }
}
