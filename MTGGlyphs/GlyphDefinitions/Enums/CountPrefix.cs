namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A bound before a number of counters: "up to" ("up to x +1/+0 counters"), "exactly" ("exactly three tide counters"), "fewer than", "at least".</summary>
public enum CountPrefix
{
    [RegexPattern("up to")]
    UpTo,

    Exactly,

    [RegexPattern("fewer than")]
    FewerThan,

    [RegexPattern("at least")]
    AtLeast
}
