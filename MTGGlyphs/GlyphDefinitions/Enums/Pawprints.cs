namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The pawprint cost of one mode of a pawprint modal spell, one to three "{p}" symbols run together before the dash: "{p}", "{p}{p}", "{p}{p}{p}".</summary>
public enum Pawprints
{
    [RegexPattern(@"\{p\}\{p\}\{p\}")]
    Three,

    [RegexPattern(@"\{p\}\{p\}")]
    Two,

    [RegexPattern(@"\{p\}")]
    One
}
