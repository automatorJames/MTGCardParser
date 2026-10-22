namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the rest": the objects left over after some were chosen or picked out earlier in the effect ("choose three permanents they control, then sacrifice the rest").</summary>
/// <exampledoc>Razia's Purification</exampledoc>
/// <examplecapture>the rest</examplecapture>
[Dependent]
public class TheRest : Glyph
{
    public override Nib[] Nibs => ["the rest"];
}
