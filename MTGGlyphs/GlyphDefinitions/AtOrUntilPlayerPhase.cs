namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(0)]
public class AtOrUntilPlayerPhase : Glyph
{
    public override Nib[] Nibs => [Prop(TemporalDisposition), "the", Prop(PhasePart), "of", Prop(Whose), Prop(Phase)];

    public TemporalDisposition TemporalDisposition { get; set; }
    public PhasePart PhasePart { get; set; }
    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}