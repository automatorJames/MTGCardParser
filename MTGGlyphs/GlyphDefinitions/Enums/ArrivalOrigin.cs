namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where a card came from when it was put into a zone: "put into a graveyard from the battlefield this turn", "put into your graveyard from anywhere this turn".</summary>
public enum ArrivalOrigin
{
    [RegexPattern("from the battlefield")]
    FromBattlefield,

    [RegexPattern("from anywhere")]
    FromAnywhere
}
