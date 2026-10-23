namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Gaining, losing or having buffs, possibly for a while, under a condition, or multiplied: "has flying", "gets +2/+2 until end of turn", "get +1/+0 and gain first strike until end of turn", "gets +1/+2 as long as you control a forest", "gets +1/+1 until end of turn for each creature blocking it beyond the first", "gets +4/+4, has flying, and is an angel in addition to its other types". A distributive "each" before it belongs to the subject (SubjectDoes). Also the second of two predicates: "can't block and has flying".</summary>
/// <exampledoc>Coral Helm</exampledoc>
/// <examplecapture>gets +2/+2 until end of turn</examplecapture>
[Dependent]
public class GainsBuffs : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Buffs), Prop(Duration), Prop(PerEach)];

    public OneOf<ManyOf<GainedOrLostBuff>, GainedOrLostBuff, ManyOf<BuffOrTypeClause>> Buffs { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
    [Optional]
    public ForEach PerEach { get; set; }
}
