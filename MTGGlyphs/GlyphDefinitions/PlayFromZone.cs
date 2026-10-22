namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A standing permission to play or cast cards from a zone other than the hand: "play lands from your graveyard", "cast creature spells from the top of your library", "play lands and cast spells from the top of your library", "cast artifact spells and colorless spells from …", "cast this card from exile", "cast that card from your graveyard". Used after "you may". A single permission is tried before a list, so "cast instant and sorcery spells" stays one.</summary>
/// <exampledoc>Crucible of Worlds</exampledoc>
/// <examplecapture>play lands from your graveyard</examplecapture>
[Dependent]
public class PlayFromZone : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Plays), "from", Prop(Source)];

    public OneOf<PlayPermission, ManyOf<PlayPermission>> Plays { get; set; }
    public OneOf<TopOfLibrary, PlayersZone, NonBattlefieldZone?> Source { get; set; }
}
