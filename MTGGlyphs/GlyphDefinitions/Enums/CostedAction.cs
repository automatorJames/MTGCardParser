namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a modified cost is paid for: "to cast" a spell or "to activate" an ability, in "costs {1} less to cast", "costs {1} less to activate".</summary>
public enum CostedAction
{
    [RegexPattern("to cast")]
    Cast,

    [RegexPattern("to activate")]
    Activate
}
