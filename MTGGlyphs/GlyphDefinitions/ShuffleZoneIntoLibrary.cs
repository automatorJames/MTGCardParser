namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Shuffling cards from a zone into a library: "shuffle your graveyard into your library", "shuffles their hand and graveyard into their library".</summary>
/// <exampledoc>Learn from the Past</exampledoc>
/// <examplecapture>shuffles their graveyard into their library</examplecapture>
[Dependent]
public class ShuffleZoneIntoLibrary : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Whose), Prop(Zones), "into", Prop(Library)];

    public ShuffleVerb Verb { get; set; }
    public Whose Whose { get; set; }
    public OneOf<ManyOf<NonBattlefieldZone?>, NonBattlefieldZone?> Zones { get; set; }
    public PlayersZone Library { get; set; }
}
