namespace MTGGlyphs.GlyphDefinitions;

public enum Possession
{
    Had,

    [RegexPattern("didn't have")]
    DidntHave,

    Has,

    [RegexPattern("doesn't have")]
    DoesntHave
}
