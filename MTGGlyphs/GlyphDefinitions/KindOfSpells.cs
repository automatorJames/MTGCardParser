namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"white spells", "goblin spells": spells of one kind, as one of a list.</summary>
/// <exampledoc>Stormscape Familiar</exampledoc>
/// <examplecapture>white spells</examplecapture>
[Dependent]
public class KindOfSpells : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), "spells"];

    public QualifierWords Qualifiers { get; set; }
}
