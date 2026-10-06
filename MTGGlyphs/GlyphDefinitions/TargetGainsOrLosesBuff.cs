namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A target gaining or losing buffs for the turn: "target creature gains flying until end of turn".</summary>
/// <exampledoc>Flying Carpet</exampledoc>
/// <examplecapture>target creature gains flying until end of turn</examplecapture>
public class TargetGainsOrLosesBuff : Glyph
{
    public override Nib[] Nibs => [Prop(TargetCard), Prop(GainedOrLostBuff), "until end of turn"];

    public TargetCard TargetCard { get; set; }
    public ManyOf<GainedOrLostBuff> GainedOrLostBuff { get; set; }
}