namespace MTGGlyphs.GlyphDefinitions;

public enum PermanentEvent
{
    Dies,

    EntersTheBattlefield,

    LeavesTheBattlefield,

    IsPutIntoAGraveyardFromTheBattlefield,

    BlocksOrBecomesBlocked,

    [RegexPattern("attacks and isn't blocked")]
    AttacksAndIsntBlocked,

    Attacks,

    Blocks,

    BecomesBlocked,

    BecomesTapped,

    IsDealtDamage,

    DealsDamage
}
