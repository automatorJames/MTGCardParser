namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Protection's reminder naming the quality by reference rather than by word, after "by anything": "of that color" ("can't be blocked, targeted, dealt damage, or enchanted by anything of that color") or "with that quality".</summary>
public enum ThatQuality
{
    [RegexPattern("of that color")]
    OfThatColor,

    [RegexPattern("with that quality")]
    WithThatQuality
}
