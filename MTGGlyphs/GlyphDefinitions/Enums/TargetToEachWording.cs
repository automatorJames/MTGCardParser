namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The two wordings of overload's reminder text after "change": the older "target" in its text to "each" and the newer its text by replacing all instances of "target" with "each".</summary>
public enum TargetToEachWording
{
    [RegexPattern("\"target\" in its text to \"each")]
    InItsText,

    [RegexPattern("its text by replacing all instances of \"target\" with \"each")]
    ReplacingAllInstances
}
