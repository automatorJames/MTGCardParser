namespace MTGGlyphs.GlyphDefinitions;

public enum CastingChoice
{
    [RegexPattern("the gift was promised")]
    GiftPromised,

    [RegexPattern("the gift wasn't promised")]
    GiftNotPromised,

    [RegexPattern("evidence was collected")]
    EvidenceCollected,

    [RegexPattern("evidence wasn't collected")]
    EvidenceNotCollected
}
