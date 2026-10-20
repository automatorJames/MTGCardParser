namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"only creatures with flying": the only creatures something may block.</summary>
/// <exampledoc>Ascending Aven</exampledoc>
/// <examplecapture>only creatures with flying</examplecapture>
[Dependent]
public class OnlyCreaturesWith : Glyph
{
    public override Nib[] Nibs => ["only creatures", Prop(With)];

    public WithQualifier With { get; set; }
}
