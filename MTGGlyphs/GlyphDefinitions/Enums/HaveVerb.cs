namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"has"/"have", as a player's life total or hand size is stated: "you have no cards in hand", "an opponent has 10 or less life", "has more life".</summary>
public enum HaveVerb
{
    [RegexPattern("has", "have")]
    Has
}
