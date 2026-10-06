namespace Glyphotype.Distiller.Agent;

/// <summary>
/// How an agent's working session runs (see <see cref="GrammarAgent.StartSessionAsync"/>): when it stops to check in
/// with the person, and what a step has to achieve to be applied. <see cref="GrammarAgent.ApplyAsync"/> enforces the
/// step rules itself, so they hold whatever the agent was told.
/// </summary>
public sealed record AgentSessionSettings
{
    /// <summary>Applied steps before stopping to summarize and wait for the person. 0 for no limit.</summary>
    public int StepsBeforeCheckIn { get; init; } = 5;

    /// <summary>Evaluations in a row without an applied step before checking in - the agent is stuck. 0 for no limit.</summary>
    public int AttemptsBeforeCheckIn { get; init; } = 10;

    /// <summary>How many bits a step must take off the total to be applied.</summary>
    public double MinimumGainBits { get; init; } = 1;

    /// <summary>Whether a step may leave any line with fewer words covered than before.</summary>
    public bool AllowLostLines { get; init; }

    /// <summary>
    /// Whether the agent is told to give every glyph a step adds or changes complete <see cref="GlyphDocumentation"/>, and
    /// reminded of any it leaves without (see <see cref="GlobalSettings.AgentDocumentsGlyphs"/>) - a reminder, never a refusal. Either way, a step changing nothing but documentation is free
    /// of the gain rule, since documenting a glyph takes no bits off.
    /// </summary>
    public bool DocumentGlyphs { get; init; }
}
