namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"spells" with qualifier words before it: "noncreature spells", "instant and sorcery spells".</summary>
/// <exampledoc>Narset, Enlightened Master</exampledoc>
/// <examplecapture>noncreature spells</examplecapture>
[Dependent]
public class QualifiedSpellsNoun : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), "spells"];

    public QualifierWords Qualifiers { get; set; }
}
