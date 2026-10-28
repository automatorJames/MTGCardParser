namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The clause closing a target-player-who-outnumbers phrase: "and is their opponent" ("target player who controls more creatures than they do and is their opponent", the Oaths) or "as you activate this ability" ("target opponent who has more life than you do as you activate this ability", the Keepers).</summary>
public enum TargetPlayerProviso
{
    [RegexPattern("and is their opponent")]
    IsTheirOpponent,

    [RegexPattern("as you activate this ability")]
    AsYouActivate
}
