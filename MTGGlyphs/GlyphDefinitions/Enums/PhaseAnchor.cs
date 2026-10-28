namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where in the turn an additional phase is inserted, said before "there is": "after this phase," "after this main phase," "after the second main phase this turn,".</summary>
public enum PhaseAnchor
{
    [RegexPattern("after this phase,")]
    AfterThisPhase,

    [RegexPattern("after this main phase,")]
    AfterThisMainPhase,

    [RegexPattern("after the second main phase this turn,")]
    AfterSecondMainPhase
}
