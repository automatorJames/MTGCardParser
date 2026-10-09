namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Gaining, losing or having buffs, possibly for a while: "has flying", "gets +2/+2 until end of turn", "gains trample and gets +x/+0".</summary>
/// <exampledoc>Coral Helm</exampledoc>
/// <examplecapture>gets +2/+2 until end of turn</examplecapture>
[Dependent]
public class GainsBuffs : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Buffs), Prop(Duration)];

    public OneOf<ManyOf<GainedOrLostBuff>, GainedOrLostBuff> Buffs { get; set; }

    [Optional]
    public UntilPhase Duration { get; set; }
}
