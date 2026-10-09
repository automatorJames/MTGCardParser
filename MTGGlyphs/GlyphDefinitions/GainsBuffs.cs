namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Gaining, losing or having buffs, possibly for a while or under a condition: "has flying", "gets +2/+2 until end of turn", "gets +1/+2 as long as you control a forest".</summary>
/// <exampledoc>Coral Helm</exampledoc>
/// <examplecapture>gets +2/+2 until end of turn</examplecapture>
[Dependent]
public class GainsBuffs : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Buffs), Prop(Duration)];

    public OneOf<ManyOf<GainedOrLostBuff>, GainedOrLostBuff> Buffs { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
