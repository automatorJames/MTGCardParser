namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"gain"/"gains" or "lose"/"loses", of life or of a quoted ability: "gain 2 life", "loses 1 life", "you would gain life", `loses "enchant creature card in a graveyard"`.</summary>
[OptionalPlural]
public enum GainOrLose
{
    Lose,
    Gain
}
