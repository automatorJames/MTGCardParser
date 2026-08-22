namespace MTGGlyphs.GlyphDefinitions;

[Color("#ff00ff")]
public class TemporaryTargetEffect : Glyph
{
    public override Nib[] Nibs => ["target", Prop(CardType), Prop(PermanentVerb), Prop(GainedOrLostBuffs), "until", Prop(Phase)];

    public CardType CardType { get; set; }
    public PermanentVerb PermanentVerb { get; set; }
    public ManyOf<Buff> GainedOrLostBuffs { get; set; }
    public Phase Phase { get; set; }
}