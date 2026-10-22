namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having done something earlier this turn, as a condition: "a planeswalker entered the battlefield under your control this turn", "a creature died under your control this turn", "this creature didn't attack this turn", "this creature dealt damage to an opponent this turn".</summary>
/// <exampledoc>Oath of Chandra</exampledoc>
/// <examplecapture>a planeswalker entered the battlefield under your control this turn</examplecapture>
[Dependent]
public class PermanentDidThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Who), Prop(Event), Prop(UnderYourControl), Prop(Turn)];

    public Subject Who { get; set; }
    public PastPermanentEvent Event { get; set; }
    [RegexPattern("under your control")]
    public bool UnderYourControl { get; set; }
    public TurnSpan Turn { get; set; }
}
