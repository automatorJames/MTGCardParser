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
    /// Whether the agent may introduce <c>[TypeFilter]</c>s on dynamic properties - and the marker interfaces they select
    /// by. When false, a change set that declares a new marker, marks a glyph with one, or filters a property by one is
    /// refused, with <see cref="GrammarAgent.TypeFilterPolicy"/> to say why; those already in the grammar may stay.
    /// </summary>
    public bool AllowTypeFilters { get; init; } = true;
}
