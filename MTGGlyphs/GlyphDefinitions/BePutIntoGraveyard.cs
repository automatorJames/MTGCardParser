namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card about to go to a graveyard, said passively after "would": "be put into a graveyard from anywhere", "be put into your graveyard", "be put into a graveyard from the battlefield". A predicate, so <see cref="WouldHappen"/> takes it.</summary>
/// <exampledoc>Rest in Peace</exampledoc>
/// <examplecapture>be put into a graveyard from anywhere</examplecapture>
[Dependent]
public class BePutIntoGraveyard : Glyph, IPredicate
{
    public override Nib[] Nibs => ["be put into", Prop(Whose), "graveyard", Prop(From)];

    public Whose Whose { get; set; }
    [Optional]
    public GraveyardOrigin? From { get; set; }
}
