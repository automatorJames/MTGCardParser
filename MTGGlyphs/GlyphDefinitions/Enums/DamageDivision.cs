namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How divided damage (or divided prevention) is split among its recipients: "divided as you choose" or "divided evenly, rounded down,".</summary>
public enum DamageDivision
{
    [RegexPattern("as you choose")]
    AsYouChoose,

    [RegexPattern("evenly, rounded down,")]
    EvenlyRoundedDown
}
