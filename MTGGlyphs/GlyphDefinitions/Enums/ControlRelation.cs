namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The relation between a player and what's theirs, in a controller phrase: "you control", "target player owns", "you own or control", "both own and control", "an opponent controls".</summary>
public enum ControlRelation
{
    [RegexPattern("owns? or controls?")]
    OwnOrControl,

    [RegexPattern("both own and control")]
    BothOwnAndControl,

    [RegexPattern("controls?")]
    Control,

    [RegexPattern("owns?")]
    Own
}
