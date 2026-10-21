namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A standing permission to play or cast cards from a zone other than the hand: "play lands from your graveyard", "cast creature spells from the top of your library", "cast this card from exile", "cast that card from your graveyard". Used after "you may".</summary>
/// <exampledoc>Crucible of Worlds</exampledoc>
/// <examplecapture>play lands from your graveyard</examplecapture>
[Dependent]
public class PlayFromZone : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(What), "from", Prop(Source)];

    public PlayVerb Verb { get; set; }
    public OneOf<OneOf<ThisCreature, ThatCard>, PlayableCards, PlayableCardType> What { get; set; }
    public OneOf<TopOfLibrary, PlayersZone, NonBattlefieldZone?> Source { get; set; }
}
