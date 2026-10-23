namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a player did (or didn't) earlier this turn, said after "who": "cast a spell this turn", "attacked with a creature this turn". The subject-less sibling of PlayerDidThisTurn.</summary>
/// <exampledoc>Angelic Arbiter</exampledoc>
/// <examplecapture>attacked with a creature this turn</examplecapture>
[Dependent]
public class PastActionThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Action), Prop(What), Prop(FromHand), Prop(Turn)];

    public PastPlayerAction Action { get; set; }
    [Optional]
    public OneOf<IndefiniteSpell, IndefinitePermanent, OneOf<PermanentsThreshold, WantedCards>> What { get; set; }
    [RegexPattern("from (your|their) hand")]
    public bool FromHand { get; set; }
    public TurnSpan Turn { get; set; }
}
