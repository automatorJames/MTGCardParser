namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a permanent would leave, in exile-instead replacements: "die" or "leave the battlefield" - "if that creature would die this turn, exile it instead", "if it would leave the battlefield, exile it instead of putting it anywhere else".</summary>
public enum DepartureEvent
{
    [RegexPattern("die")]
    Die,

    [RegexPattern("leave the battlefield")]
    LeaveTheBattlefield
}
