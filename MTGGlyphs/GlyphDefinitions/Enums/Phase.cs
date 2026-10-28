namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A step, phase or turn as a point in time: "untap step", "upkeep" ("upkeep step"), "draw step", "first main phase" ("precombat main phase"), "combat" ("combat phase"), "declare blockers step", "combat damage step", "end step", "end of turn", "your next turn", "turn" ("at the beginning of your upkeep", "until end of turn").</summary>
public enum Phase
{
    UntapStep,

    [RegexPattern("upkeep step", "upkeep")]
    Upkeep,

    DrawStep,

    [RegexPattern("first main phase", "precombat main phase")]
    FirstMainPhase,

    [RegexPattern("second main phase", "postcombat main phase")]
    SecondMainPhase,

    MainPhase,

    [RegexPattern("combat damage step")]
    CombatDamageStep,

    [RegexPattern("end of combat step", "end of combat")]
    EndOfCombatStep,

    [RegexPattern("combat phase", "combat")]
    CombatPhase,

    CombatStep,

    DeclareAttackersStep,

    DeclareBlockersStep,

    DamageStep,

    EndStep,

    CleanupStep,

    EndOfTurn,

    [RegexPattern("your next turn")]
    YourNextTurn,

    NextTurn,

    Turn
}
