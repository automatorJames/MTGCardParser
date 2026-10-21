namespace MTGGlyphs.GlyphDefinitions;

public enum TargetToEachWording
{
    [RegexPattern("\"target\" in its text to \"each")]
    InItsText,

    [RegexPattern("its text by replacing all instances of \"target\" with \"each")]
    ReplacingAllInstances
}
