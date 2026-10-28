namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The event whose triggers a trigger-doubler (Panharmonicon, Teysa Karlov) counts: "entering the battlefield" or "entering", "dying", or "attacking", as in "if a permanent entering causes a triggered ability of a permanent you control to trigger".</summary>
public enum TriggeringEvent
{
    [RegexPattern("entering the battlefield", "entering")]
    Entering,

    [RegexPattern("dying")]
    Dying,

    [RegexPattern("attacking")]
    Attacking
}
