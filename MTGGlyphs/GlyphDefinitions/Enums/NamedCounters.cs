namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters referred back to after a condition counted them, as the object of "remove": "them", "all of them", "them all" ("if it has three or more ritual counters on it, remove them").</summary>
public enum NamedCounters
{
    [RegexPattern("all of them", "them all")]
    AllOfThem,

    [RegexPattern("them")]
    Them
}
