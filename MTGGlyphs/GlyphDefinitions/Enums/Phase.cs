namespace MTGGlyphs.GlyphDefinitions;

public enum Phase
{
    UntapStep,

    Upkeep,

    DrawStep,

    MainPhase,

    [RegexPattern("combat damage step")]
    CombatDamageStep,

    [RegexPattern("end of combat step")]
    EndOfCombatStep,

    [RegexPattern("combat phase", "combat")]
    CombatPhase,

    CombatStep,

    DeclareAttackersStep,

    DeclareBlockersStep,

    DamageStep,

    EndStep,

    EndOfTurn,

    Turn
}
