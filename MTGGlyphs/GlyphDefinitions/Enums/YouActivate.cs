namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who activates the abilities a cost change applies to, after "abilities": "you activate" ("equip abilities you activate") or "your opponents activate" ("abilities your opponents activate").</summary>
public enum YouActivate
{
    [RegexPattern("you activate")]
    YouActivate,

    [RegexPattern("your opponents activate")]
    OpponentsActivate
}
