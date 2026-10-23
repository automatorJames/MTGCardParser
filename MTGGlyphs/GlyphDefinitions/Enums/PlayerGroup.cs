namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerGroup
{
    [RegexPattern("one or more opponents")]
    OneOrMoreOpponents,

    [RegexPattern("one or more of your opponents")]
    OneOrMoreOfYourOpponents,

    [RegexPattern("one or more players")]
    OneOrMorePlayers
}
