namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's attacking", "it's untapped", "it's not attacking", "it's enchanted": the object just named in, or out of, a state. Being a kind ("it's a creature", "it's a human") is ObjectIsKind.</summary>
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
