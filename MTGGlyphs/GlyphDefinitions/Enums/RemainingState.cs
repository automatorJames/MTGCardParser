namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A state a permanent or card stays in, after "remains" or "remain": "tapped", "on the battlefield", "exiled" ("{this} remains tapped", "it remains exiled").</summary>
public enum RemainingState
{
    Tapped,

    [RegexPattern("on the battlefield")]
    OnTheBattlefield,

    Exiled
}
