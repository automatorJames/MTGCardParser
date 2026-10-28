namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several players at once as the subject of a trigger event: "one or more opponents", "one or more of your opponents", "one or more players" ("whenever one or more opponents are dealt noncombat damage, ...").</summary>
public enum PlayerGroup
{
    [RegexPattern("one or more opponents")]
    OneOrMoreOpponents,

    [RegexPattern("one or more of your opponents")]
    OneOrMoreOfYourOpponents,

    [RegexPattern("one or more players")]
    OneOrMorePlayers
}
