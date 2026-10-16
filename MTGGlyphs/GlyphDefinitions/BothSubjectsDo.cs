namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two subject-and-action clauses joined by "and": "target player loses 2 life and you gain 2 life", "you draw a card and you lose 1 life".</summary>
/// <exampledoc>Syphon Life</exampledoc>
/// <examplecapture>target player loses 2 life and you gain 2 life</examplecapture>
public class BothSubjectsDo : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second)];

    public SubjectDoes First { get; set; }
    public SubjectDoes Second { get; set; }
}
