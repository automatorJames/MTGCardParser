namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A reference back to a name chosen earlier by the ability, after "with": "with the chosen name", "with the chosen names", "with that name" (Meddling Mage, Pithing Needle).</summary>
public enum ChosenNameReference
{
    [RegexPattern("the chosen names?")]
    TheChosenName,

    [RegexPattern("that name")]
    ThatName
}
