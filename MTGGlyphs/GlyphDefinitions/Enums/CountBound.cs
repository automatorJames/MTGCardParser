namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The direction of an open-ended count: "more", or "fewer"/"less", in "controls more lands than you", "3 or more damage", "two or fewer other lands", "twenty or less cards in it".</summary>
public enum CountBound
{
    [RegexPattern("more")]
    AtLeast,

    [RegexPattern("fewer", "less")]
    AtMost
}
