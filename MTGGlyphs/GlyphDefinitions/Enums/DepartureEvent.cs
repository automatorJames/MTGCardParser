namespace MTGGlyphs.GlyphDefinitions;

public enum DepartureEvent
{
    [RegexPattern("die")]
    Die,

    [RegexPattern("leave the battlefield")]
    LeaveTheBattlefield
}
