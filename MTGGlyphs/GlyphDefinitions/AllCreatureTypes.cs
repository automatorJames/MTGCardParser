namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Every creature type at once, as something a creature gains or loses: "gains all creature types", "loses all creature types" (changelings and Velis Vel).</summary>
/// <exampledoc>Amoeboid Changeling</exampledoc>
/// <examplecapture>all creature types</examplecapture>
[Dependent]
[RegexPattern("all creature types")]
public class AllCreatureTypes : Glyph;
