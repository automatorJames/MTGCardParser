namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose abilities: "of artifacts", "of an elemental", "of colorless eldrazi", "of an artifact source", "of creature sources", "of a source of the chosen type", "of a creature source of the chosen type".</summary>
/// <exampledoc>Renowned Weaponsmith</exampledoc>
/// <examplecapture>of artifacts</examplecapture>
[Dependent]
public class AbilitiesOf : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Source), Prop(AsSource), Prop(Chosen)];

    public OneOf<Permanents, IndefinitePermanent, IndefiniteDeterminer?> Source { get; set; }
    [RegexPattern("sources?")]
    public bool AsSource { get; set; }
    [Optional]
    public ChosenAspect? Chosen { get; set; }
}
