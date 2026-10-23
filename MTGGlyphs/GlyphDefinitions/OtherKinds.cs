namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"other", "other instant and sorcery": the kinds of other spells or cards counted, besides this one.</summary>
/// <exampledoc>Storm Entity</exampledoc>
/// <examplecapture>other</examplecapture>
[Dependent]
public class OtherKinds : Glyph
{
    public override Nib[] Nibs => ["other", Prop(Qualifiers)];

    [Optional]
    public QualifierWords Qualifiers { get; set; }
}
