namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The state a permanent enters the battlefield in: "tapped", "untapped", "tapped and attacking", "face down" - "enters tapped", "onto the battlefield tapped and attacking".</summary>
public enum EntryState
{
    [RegexPattern("tapped and attacking")]
    TappedAndAttacking,

    Tapped,

    Untapped,

    [RegexPattern("face down")]
    FaceDown
}
