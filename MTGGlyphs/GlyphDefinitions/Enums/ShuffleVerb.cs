namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb "shuffle" or "shuffles" heading a shuffle action: "shuffle those cards into your library", "that player shuffles their graveyard into their library".</summary>
public enum ShuffleVerb
{
    [RegexPattern("shuffles?")]
    Shuffle
}
