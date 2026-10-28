namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb of a reveal-until loop and of its stop condition, "reveal" or "exile": "reveal cards from the top of your library until you reveal a creature card", "exile cards from the top of your library until you exile a nonland card".</summary>
public enum RevealOrExile
{
    [RegexPattern("reveals?")]
    Reveal,

    [RegexPattern("exiles?")]
    Exile
}
