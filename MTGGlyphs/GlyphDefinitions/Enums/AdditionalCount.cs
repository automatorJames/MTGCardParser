namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many extra phases an effect adds: "there is an additional combat phase", "there are two additional combat phases".</summary>
public enum AdditionalCount
{
    [RegexPattern("an additional")]
    One,

    [RegexPattern("two additional")]
    Two
}
