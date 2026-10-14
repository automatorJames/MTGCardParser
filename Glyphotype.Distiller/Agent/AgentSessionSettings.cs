namespace Glyphotype.Distiller.Agent;

/// <summary>
/// How an agent's working session runs (see <see cref="GrammarAgent.StartSessionAsync"/>): when it stops to check in
/// with the person. What a step has to achieve to be applied isn't a setting: <see cref="GrammarAgent.ApplyAsync"/>
/// enforces the step rules itself, so they hold whatever the agent was told.
/// </summary>
public sealed record AgentSessionSettings
{
    /// <summary>Applied steps before stopping to summarize and wait for the person. 0 for no limit.</summary>
    public int StepsBeforeCheckIn { get; init; } = 5;

    /// <summary>Evaluations in a row without an applied step before checking in - the agent is stuck. 0 for no limit.</summary>
    public int AttemptsBeforeCheckIn { get; init; } = 10;

    /// <summary>How many words the workspace's journal may come to: past it, an agent must prune before adding. 0 for no limit.</summary>
    public int JournalWordLimit { get; init; } = 1500;

    /// <summary>
    /// Whether the agent is told to give every glyph a step adds or changes complete <see cref="GlyphDocumentation"/>, and
    /// reminded of any it leaves without (see <see cref="GlobalSettings.AgentDocumentsGlyphs"/>) - a reminder, never a refusal. Either way, a step changing nothing but documentation is free
    /// of the step rules, since documenting a glyph covers nothing.
    /// </summary>
    public bool DocumentGlyphs { get; init; }
}
