namespace MTGGlyphs.GlyphDefinitions;

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

    EndOfTurn,

    [RegexPattern("your next turn")]
    YourNextTurn,

    NextTurn,

    Turn
}
