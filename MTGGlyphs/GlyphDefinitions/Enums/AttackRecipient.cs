namespace MTGGlyphs.GlyphDefinitions;

public enum AttackRecipient
{
    [RegexPattern("you or planeswalkers you control")]
    YouOrYourPlaneswalkers,

    You,

    [RegexPattern("a player")]
    APlayer
}
