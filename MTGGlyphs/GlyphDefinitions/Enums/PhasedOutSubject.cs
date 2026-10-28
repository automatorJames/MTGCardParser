namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The subject of the phased-out reminder, contracted with its verb: "it's" ("while it's phased out"), "they're" ("while they're phased out"), "permanents are" ("while permanents are phased out").</summary>
public enum PhasedOutSubject
{
    [RegexPattern("it's")]
    It,

    [RegexPattern("they're")]
    They,

    [RegexPattern("permanents are")]
    Permanents
}
