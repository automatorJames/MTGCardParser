namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A copy that enters with counters: "except it enters with an additional +1/+1 counter on it".</summary>
/// <exampledoc>Spark Double</exampledoc>
/// <examplecapture>except it enters with an additional +1/+1 counter on it</examplecapture>
[Dependent]
public class CopyEntersWithCounters : Glyph
{
    public override Nib[] Nibs => ["except it enters", Prop(With)];

    public EntersWithCounters With { get; set; }
}
