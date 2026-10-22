namespace MTGGlyphs.GlyphDefinitions;

public enum ControlledPeriod
{
    Turn,

    [RegexPattern("combat phase")]
    CombatPhase
}
