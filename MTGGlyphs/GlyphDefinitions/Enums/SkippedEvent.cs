namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A pending event skipped by reference rather than by name, after "skip": "that draw" ("skip that draw") or "that turn" ("skips that turn").</summary>
public enum SkippedEvent
{
    [RegexPattern("that draw")]
    ThatDraw,

    [RegexPattern("that turn")]
    ThatTurn
}
