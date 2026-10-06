namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Someone sacrificing the permanent just mentioned: "that creature's controller sacrifices it".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>that creature's controller sacrifices it</examplecapture>
public class SacrificeIt : Glyph
{
    public override Nib[] Nibs => [Prop(Who), Pattern("sacrifice(s)?"), "it"];

    public Who? Who { get; set; }
}