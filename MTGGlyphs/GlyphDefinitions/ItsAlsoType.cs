namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's also a zombie", "it's also an orc": the object just named gains a subtype on top of its others.</summary>
/// <exampledoc>Lazotep Reaver</exampledoc>
/// <examplecapture>it's also a zombie</examplecapture>
public class ItsAlsoType : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "also", Pattern("an?"), Prop(Kind)];

    public ItIs Subject { get; set; }
    public PermanentKind Kind { get; set; }
}
