namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player having had permanents enter under their control this turn (or last turn), as a condition: "an opponent had two or more lands enter the battlefield under their control this turn", "you had a land enter the battlefield under your control this turn", "you had another creature enter the battlefield under your control last turn".</summary>
/// <exampledoc>Lavaball Trap</exampledoc>
/// <examplecapture>an opponent had two or more lands enter the battlefield under their control this turn</examplecapture>
[Dependent]
public class PlayerHadPermanentsEnter : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "had", Prop(What), "enter the battlefield under", Alt("their", "your"), "control", Prop(Turn)];

    public OneOf<WhichPlayer?, PlayerReference, PerfectSubject?> Player { get; set; }
    public OneOf<PermanentsThreshold, IndefinitePermanent> What { get; set; }
    public TurnSpan Turn { get; set; }
}
