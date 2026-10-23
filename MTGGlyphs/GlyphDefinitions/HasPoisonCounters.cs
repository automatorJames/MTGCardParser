namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Having a number of poison counters, as what picks out players after "who": "who has three or more poison counters" (corrupted).</summary>
/// <exampledoc>Feed the Infection</exampledoc>
/// <examplecapture>has three or more poison counters</examplecapture>
[Dependent]
public class HasPoisonCounters : Glyph
{
    public override Nib[] Nibs => [Pattern("(has|have)"), Prop(Count), "poison counters"];

    public AtLeastCount Count { get; set; }
}
