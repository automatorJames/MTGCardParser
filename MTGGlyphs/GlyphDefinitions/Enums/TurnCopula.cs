namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's" or its negation, "it's not" or "it isn't", opening a whose-turn condition: "it's your turn", "it's not your turn", "it isn't your main phase".</summary>
public enum TurnCopula
{
    [RegexPattern("it's")]
    Is,

    [RegexPattern("it's not", "it isn't")]
    IsNot
}
