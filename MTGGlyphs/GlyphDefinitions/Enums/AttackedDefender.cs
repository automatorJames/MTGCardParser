namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a player attacks, between "attack" and "with" in a trigger condition: "you attack a player with one or more equipped creatures", "you attack a player or planeswalker with …".</summary>
public enum AttackedDefender
{
    [RegexPattern("a player or planeswalker")]
    PlayerOrPlaneswalker,

    [RegexPattern("a player")]
    Player
}
