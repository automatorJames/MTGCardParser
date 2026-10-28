namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether an optional choice offered as a spell was cast was taken, as a condition on its effect: "the gift was promised" / "the gift wasn't promised" (gift), "evidence was collected" / "evidence wasn't collected" (collect evidence).</summary>
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
