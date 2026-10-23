namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The life a player gained or lost this turn, as an amount: "the amount of life you gained this turn", "the total amount of life your opponents lost this turn", "the amount of life you gained", "the amount of life you've gained this turn", "the life they lost this turn".</summary>
/// <exampledoc>Accomplished Alchemist</exampledoc>
/// <examplecapture>the amount of life you gained this turn</examplecapture>
[Dependent]
public class LifeChangedAmount : Glyph
{
    public override Nib[] Nibs => [Pattern("the (?:(?:total )?amount of )?life"), Prop(Player), Prop(Change), Prop(Turn)];

    public OneOf<WhichPlayer?, OneOf<PlayerReference, TheyPlayer>, PerfectSubject?> Player { get; set; }
    public LifeChange Change { get; set; }
    [Optional]
    public TurnSpan? Turn { get; set; }
}
