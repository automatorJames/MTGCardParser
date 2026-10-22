namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player barred from a game action other than casting or gaining life: "can't draw cards", "can't draw more than one card each turn", "can't search libraries", "can't get counters", "can't get poison counters", "can't cycle cards", optionally for a time ("this turn").</summary>
/// <exampledoc>Leonin Arbiter</exampledoc>
/// <examplecapture>can't search libraries</examplecapture>
[Dependent]
public class PlayerCantAct : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't", Prop(Action), Prop(Duration)];

    public PlayerProhibition Action { get; set; }
    [Optional]
    public OneOf<ThisPeriod, EffectDuration> Duration { get; set; }
}
