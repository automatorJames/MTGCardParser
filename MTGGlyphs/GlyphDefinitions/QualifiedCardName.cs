namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"an artifact card name", "a nonartifact, nonland card name": the name of a card of a kind, to be chosen.</summary>
/// <exampledoc>Dispossess</exampledoc>
/// <examplecapture>an artifact card name</examplecapture>
[Dependent]
public class QualifiedCardName : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Qualifiers), "card name"];

    public QualifierWords Qualifiers { get; set; }
}
