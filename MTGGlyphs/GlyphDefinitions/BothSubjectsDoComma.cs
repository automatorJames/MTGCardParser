namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two subject-and-action clauses joined by ", and" - the comma form of BothSubjectsDo, kept apart because a dynamic predicate needs a literal after it to end: "enchanted creature can't attack or block, and its activated abilities can't be activated".</summary>
/// <exampledoc>Arachnus Web</exampledoc>
/// <examplecapture>enchanted creature can't attack or block, and its activated abilities can't be activated</examplecapture>
public class BothSubjectsDoComma : Glyph
{
    public override Nib[] Nibs => [Prop(First), ", and", Prop(Second)];

    public SubjectDoes First { get; set; }
    public SubjectDoes Second { get; set; }
}
