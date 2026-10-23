namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards a player owns across zones, to count: "instant and sorcery cards you own in exile and in your graveyard", "cards you own in exile".</summary>
/// <exampledoc>Crackling Drake</exampledoc>
/// <examplecapture>instant and sorcery cards you own in exile and in your graveyard</examplecapture>
[Dependent]
public class OwnedCardsInZones : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("card"), Prop(Owner), Alt("own", "owns"), Prop(Zones)];

    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    public WhichPlayer Owner { get; set; }
    public OneOf<ManyOf<InZone>, InZone> Zones { get; set; }
}
