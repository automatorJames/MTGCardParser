namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two permanents acted on together, each its own phrase: "target creature and target land", "this creature and target creature without flying that's attacking you", "up to one target artifact you control and up to one target creature you control".</summary>
/// <exampledoc>Fumarole</exampledoc>
/// <examplecapture>target creature and target land</examplecapture>
[Dependent]
public class PermanentPair : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second)];

    public PermanentPhrase First { get; set; }
    public PermanentPhrase Second { get; set; }
}
