namespace MTGGlyphs.GlyphDefinitions;

public enum CopyingVerb
{
    [RegexPattern("becomes?")]
    Becomes,

    [RegexPattern("enters? as")]
    EntersAs,

    [RegexPattern("enters? tapped as")]
    EntersTappedAs
}
