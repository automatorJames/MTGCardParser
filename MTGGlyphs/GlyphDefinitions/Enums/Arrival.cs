namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether cards arrived in their zone recently, opening a recency qualifier on cards outside the battlefield: "that was put there from the battlefield this turn", "that were put there from anywhere this turn", "that wasn't put there this turn".</summary>
public enum Arrival
{
    [RegexPattern("that (?:was|were) put there")]
    PutThere,

    [RegexPattern("that (?:wasn't|weren't) put there")]
    NotPutThere
}
