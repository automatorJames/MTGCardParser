namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object having, or not having, a keyword, as a state: "it has defender", "equipped creature has defender", "it doesn't have suspend", "it doesn't have first strike".</summary>
/// <exampledoc>Manor Gargoyle</exampledoc>
/// <examplecapture>it has defender</examplecapture>
[Dependent]
public class ObjectHasKeyword : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Possession), Prop(Keywords)];

    public PermanentPhrase Subject { get; set; }
    public Possession Possession { get; set; }
    public OneOf<ManyOf<Keyword?>, Keyword?> Keywords { get; set; }
}
