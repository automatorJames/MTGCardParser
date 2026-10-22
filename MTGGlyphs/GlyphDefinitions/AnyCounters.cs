namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters of any kind and number, unqualified: "if it had counters on it".</summary>
/// <exampledoc>Hardened Scales</exampledoc>
/// <examplecapture>counters</examplecapture>
[Dependent]
[RegexPattern("counters")]
public class AnyCounters : Glyph;
