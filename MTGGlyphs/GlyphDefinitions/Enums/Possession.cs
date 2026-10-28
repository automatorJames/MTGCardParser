namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether an object has, or had, a keyword, present or past, positive or negative: "has", "doesn't have", "had", "didn't have" ("it has defender", "as though it didn't have defender", "as though it had haste").</summary>
public enum Possession
{
    Had,

    [RegexPattern("didn't have")]
    DidntHave,

    Has,

    [RegexPattern("doesn't have")]
    DoesntHave
}
