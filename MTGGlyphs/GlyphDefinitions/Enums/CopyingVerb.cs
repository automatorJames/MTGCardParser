namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb by which a permanent takes on another object's copiable values: "becomes" a copy of (copy effects), "enters as" a copy of (clones), "enters tapped as" a copy of.</summary>
public enum CopyingVerb
{
    [RegexPattern("becomes?")]
    Becomes,

    [RegexPattern("enters? as")]
    EntersAs,

    [RegexPattern("enters? tapped as")]
    EntersTappedAs
}
