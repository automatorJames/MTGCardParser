namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where a clone finds what it copies: "on the battlefield" or "card in a graveyard", in "any creature on the battlefield", "any creature card in a graveyard".</summary>
public enum CopySourceZone
{
    [RegexPattern("on the battlefield")]
    OnTheBattlefield,

    [RegexPattern("card in a graveyard")]
    CardInAGraveyard
}
