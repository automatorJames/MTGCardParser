namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a player did something to an object of a kind as the effect just described let them: "you discard a creature card this way", "you sacrifice an island this way", "that player discards an artifact card this way", "you exiled a land card this way", "you cast a spell this way", "you reveal a land card this way", "you discard one or more nonland cards this way".</summary>
/// <exampledoc>Serendib Djinn</exampledoc>
/// <examplecapture>you sacrifice an island this way</examplecapture>
[Dependent]
public class PlayerActedThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Action), Prop(Object), "this way"];

    public OneOf<WhichPlayer?, PlayerReference> Player { get; set; }
    public PlayerThisWayAction Action { get; set; }
    public OneOf<IndefinitePermanent, IndefiniteCard, ASpell, CountedCards> Object { get; set; }
}
