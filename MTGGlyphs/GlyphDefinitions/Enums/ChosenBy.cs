namespace MTGGlyphs.GlyphDefinitions;

public enum ChosenBy
{
    [RegexPattern("of your choice")]
    You,

    [RegexPattern("of an opponent's choice")]
    AnOpponent,

    [RegexPattern("of target opponent's choice")]
    TargetOpponent
}
