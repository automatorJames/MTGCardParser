namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many attackers a creature may block beyond the usual one: "can block an additional creature each combat", "can block any number of creatures this turn".</summary>
public enum BlockCount
{
    [RegexPattern("an additional creature")]
    AnAdditionalCreature,

    [RegexPattern("any number of creatures")]
    AnyNumberOfCreatures
}
