namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The connective that introduces a condition on the game state: "as long as you control a forest", "unless defending player controls an island", "if you have 25 or more life".</summary>
public enum ConditionConnective
{
    [RegexPattern("as long as")]
    AsLongAs,

    Unless,

    If
}
