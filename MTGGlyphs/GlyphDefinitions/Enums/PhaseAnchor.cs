namespace MTGGlyphs.GlyphDefinitions;

public enum PhaseAnchor
{
    [RegexPattern("after this phase,")]
    AfterThisPhase,

    [RegexPattern("after this main phase,")]
    AfterThisMainPhase,

    [RegexPattern("after the second main phase this turn,")]
    AfterSecondMainPhase
}
