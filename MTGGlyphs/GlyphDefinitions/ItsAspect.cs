namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's attacking", "it's untapped", "it's not attacking": the object just named in, or out of, a state. Being a type ("it's a creature") is TransformedType.</summary>
/// <exampledoc>Kor Scythemaster</exampledoc>
/// <examplecapture>it's attacking</examplecapture>
[Dependent]
public class ItsAspect : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Negated), Prop(Aspect)];

    public ItIs Subject { get; set; }
    [RegexPattern("not")]
    public bool Negated { get; set; }
    public CardAspect Aspect { get; set; }
}
