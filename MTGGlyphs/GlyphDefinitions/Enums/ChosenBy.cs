namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who picks a card from a zone, as a qualifier on it: "card of your choice", "card of an opponent's choice from your graveyard", "card of target opponent's choice".</summary>
public enum ChosenBy
{
    [RegexPattern("of your choice")]
    You,

    [RegexPattern("of an opponent's choice")]
    AnOpponent,

    [RegexPattern("of target opponent's choice")]
    TargetOpponent
}
