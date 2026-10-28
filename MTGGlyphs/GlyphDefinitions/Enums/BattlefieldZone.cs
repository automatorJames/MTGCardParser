namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the battlefield" as the destination named first in a return: "returns to the battlefield all artifact, creature, enchantment, and land cards in their graveyard …" (where the alternative is a player's zone, "return to your hand …").</summary>
public enum BattlefieldZone
{
    [RegexPattern("the battlefield")]
    Battlefield
}
