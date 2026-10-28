namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"or more"/"or greater" or "or fewer"/"or less" after a number of cards in hand: "seven or more cards in hand", "one or fewer cards in hand".</summary>
public enum CountComparison
{
    [RegexPattern("or more", "or greater")]
    OrMore,

    [RegexPattern("or fewer", "or less")]
    OrFewer
}
