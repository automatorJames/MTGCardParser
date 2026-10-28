namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb heading a removal action, "destroy" or "exile" (also "destroys", "exiles", "exiling"): "destroy target land", "exile target spell with mana value 4 or less".</summary>
public enum RemovalVerb
{
    [RegexPattern("destroys?")]
    Destroy,

    [RegexPattern("exiles?", "exiling")]
    Exile
}
