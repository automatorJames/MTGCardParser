namespace MTGGlyphs.GlyphDefinitions;

public enum CounterAction
{
    [RegexPattern("puts?")]
    Put,

    [RegexPattern("removes?")]
    Remove
}
