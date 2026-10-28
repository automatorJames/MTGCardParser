namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a spell was cast from a zone or not: "was" or "wasn't" in "this spell was cast from a graveyard", "this spell wasn't cast from your hand".</summary>
public enum PastPolarity
{
    Was,

    [RegexPattern("wasn't")]
    Wasnt
}
