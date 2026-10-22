namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A single source of damage, as replacement and prevention effects name one: "a source", "a red source", "a source an opponent controls", "another source you control", "an instant or sorcery source you control", "any source", or a spell as the source, "a red instant or sorcery spell you control".</summary>
/// <exampledoc>Guardian Seraph</exampledoc>
/// <examplecapture>a source an opponent controls</examplecapture>
[Dependent]
public class ASource : Glyph
{
    public override Nib[] Nibs => [Pattern("(an?|another|any)"), Prop(Qualifiers), Prop(Noun), Prop(Controller)];

    [Optional]
    public QualifierWords Qualifiers { get; set; }
    public SourceNoun Noun { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
