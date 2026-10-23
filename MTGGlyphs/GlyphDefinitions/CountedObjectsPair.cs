namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two groups counted together by one "for each": "creature you control and each creature card in your graveyard", "other zombie you control and each zombie card in your graveyard".</summary>
/// <exampledoc>Moon-Vigil Adherents</exampledoc>
/// <examplecapture>creature you control and each creature card in your graveyard</examplecapture>
[Dependent]
public class CountedObjectsPair : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and each", Prop(Second)];

    public CountedObjects First { get; set; }
    public OneOf<CountedObjects, OwnedCardsInZones> Second { get; set; }
}
