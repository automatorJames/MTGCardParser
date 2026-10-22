namespace MTGGlyphs.GlyphDefinitions;

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
