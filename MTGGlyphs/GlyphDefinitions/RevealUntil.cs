namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Revealing (or exiling) cards from the top of a library one at a time until a card of some kind turns up: "reveal cards from the top of your library until you reveal a creature card". A predicate, so subjects, "you may" and "then" sequences take it.</summary>
/// <exampledoc>Balustrade Spy</exampledoc>
/// <examplecapture>reveals cards from the top of their library until they reveal a land card</examplecapture>
public class RevealUntil : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), "cards from the top of", Prop(Library), "until", Prop(StopCondition)];

    public RevealOrExile Verb { get; set; }
    public PlayersZone Library { get; set; }
    public OneOf<PlayerFindsCard, CardIsFound> StopCondition { get; set; }
}
