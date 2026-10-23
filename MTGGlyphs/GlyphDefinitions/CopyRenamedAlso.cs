namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The second change after a copy's new name: "and it's legendary in addition to its other types", "and he's a legendary human mercenary villain creature".</summary>
/// <exampledoc>Sarkhan, Soul Aflame</exampledoc>
/// <examplecapture>and it's legendary in addition to its other types</examplecapture>
[Dependent]
public class CopyRenamedAlso : Glyph
{
    public override Nib[] Nibs => ["and", Prop(Pronoun), "'s", Prop(Kind)];

    public CopyPronoun Pronoun { get; set; }
    public CopyKind Kind { get; set; }
}
