namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of each of those kinds of counters" or "of those counters": the same kinds as the counters being replaced.</summary>
/// <exampledoc>Winding Constrictor</exampledoc>
/// <examplecapture>of each of those kinds of counters</examplecapture>
[Dependent]
public class EachOfThoseKindsOfCounters : Glyph
{
    public override Nib[] Nibs => [Alt("of each of those kinds of counters", "of those counters")];
}
