namespace MTGGlyphs.GlyphDefinitions;

public class TargetGainsOrLosesBuff : Glyph
{
    public override Nib[] Nibs => [Prop(TargetCard), Prop(GainedOrLostBuff), "until end of turn"];

    public TargetCard TargetCard { get; set; }
    public ManyOf<GainedOrLostBuff> GainedOrLostBuff { get; set; }
}