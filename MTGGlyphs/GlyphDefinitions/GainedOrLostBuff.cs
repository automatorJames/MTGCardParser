namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A verb and the buff it gives: "gets +1/+1", "has flying".</summary>
/// <exampledoc>Blessing</exampledoc>
/// <examplecapture>gets +1/+1</examplecapture>
[Dependent]
public class GainedOrLostBuff : Glyph
{
    public PermanentVerb PermanentVerb { get; set; }
    public Buff Buff { get; set; }
}