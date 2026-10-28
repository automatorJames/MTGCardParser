namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whom a creature attacks, after "attack" or "attacking": "can't attack you", "can't attack you or planeswalkers you control", "that's attacking you", "attack a player".</summary>
public enum AttackRecipient
{
    [RegexPattern("you or planeswalkers you control")]
    YouOrYourPlaneswalkers,

    You,

    [RegexPattern("a player")]
    APlayer
}
