namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many targets a spell has, between "that targets" and what it targets: "only a single" ("that targets only a single creature"), "only", or "one or more" ("that targets one or more creatures").</summary>
public enum SpellTargetCount
{
    [RegexPattern("only a single")]
    OnlyASingle,

    Only,

    [RegexPattern("one or more")]
    OneOrMore
}
