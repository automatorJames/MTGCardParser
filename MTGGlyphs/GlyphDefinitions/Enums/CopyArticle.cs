namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What opens a copy's changed description, with the article when the copy becomes a described thing: "except it's a 4/4 black zombie jackal warrior", "except it's 7/7", "and it's an enchantment in addition to its other types" (continuing a list of changes).</summary>
public enum CopyArticle
{
    [RegexPattern("except it's an?", "except it's")]
    ItIs,

    [RegexPattern("and it's an?", "and it's")]
    AndItIs
}
