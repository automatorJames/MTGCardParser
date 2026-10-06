namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A target given buffs until a point in the turn: "target creature gets +2/+2 until end of turn".</summary>
/// <exampledoc>Coral Helm</exampledoc>
/// <examplecapture>target creature gets +2/+2 until end of turn</examplecapture>
public class TemporaryTargetEffect : Glyph
{
    public override Nib[] Nibs => ["target", Prop(CardType), Prop(PermanentVerb), Prop(GainedOrLostBuffs), "until", Prop(Phase)];

    public CardType CardType { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public ManyOf<Buff> GainedOrLostBuffs { get; set; }
    public Phase Phase { get; set; }
}