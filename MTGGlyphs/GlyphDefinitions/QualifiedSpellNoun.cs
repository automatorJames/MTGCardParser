namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"instant or sorcery spell" without an article, as after "your next".</summary>
/// <exampledoc>Twinferno</exampledoc>
/// <examplecapture>instant or sorcery spell</examplecapture>
[Dependent]
public class QualifiedSpellNoun : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), "spell"];

    public QualifierWords Qualifiers { get; set; }
}
