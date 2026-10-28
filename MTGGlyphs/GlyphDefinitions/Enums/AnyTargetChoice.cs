namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The word before "target" in a player-or-permanent target: "any target", "any other target", or in a list of several, "another target", "a second target", "a third target".</summary>
public enum AnyTargetChoice
{
    Any,

    [RegexPattern("any other")]
    AnyOther,

    Another,

    [RegexPattern("a second")]
    ASecond,

    [RegexPattern("a third")]
    AThird
}
