namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player having done (or not done) something earlier this turn, as a condition: "you didn't cast a spell this turn", "that player didn't cast a creature spell this turn", "an opponent gained life this turn", "you played a land this turn", "you haven't cast a spell from your hand this turn", "you've cast another blue spell this turn".</summary>
/// <exampledoc>Predatory Advantage</exampledoc>
/// <examplecapture>that player didn't cast a creature spell this turn</examplecapture>
[Dependent]
public class PlayerDidThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Action), Prop(What), Prop(FromHand), Prop(Turn)];

    public OneOf<WhichPlayer?, PlayerReference, PerfectSubject?> Player { get; set; }
    public PastPlayerAction Action { get; set; }
    [Optional]
    public OneOf<IndefiniteSpell, IndefinitePermanent> What { get; set; }
    [RegexPattern("from (your|their) hand")]
    public bool FromHand { get; set; }
    public TurnSpan Turn { get; set; }
}
