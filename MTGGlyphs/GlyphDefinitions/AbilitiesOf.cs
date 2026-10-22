namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose abilities: "of artifacts", "of an elemental", "of colorless eldrazi", "of an artifact source", "of creature sources".</summary>
/// <exampledoc>Renowned Weaponsmith</exampledoc>
/// <examplecapture>of artifacts</examplecapture>
[Dependent]
public class AbilitiesOf : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Source), Prop(AsSource)];

    public OneOf<Permanents, IndefinitePermanent> Source { get; set; }
    [RegexPattern("sources?")]
    public bool AsSource { get; set; }
}
