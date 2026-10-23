namespace MTGGlyphs.GlyphDefinitions;

public enum ChosenNameReference
{
    [RegexPattern("the chosen names?")]
    TheChosenName,

    [RegexPattern("that name")]
    ThatName
}
