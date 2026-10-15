namespace Glyphotype.Distiller.Agent;

/// <summary>
/// What a session weighs most in choosing its targets and steps - a leaning within the loop the guide describes, never a
/// change to the step rules, which <see cref="GrammarAgent.ApplyAsync"/> enforces whatever the focus.
/// </summary>
public enum AgentFocus
{
    /// <summary>The guide's own balance: coverage first, bits deciding how.</summary>
    Default,

    /// <summary>The most words covered per step, at some cost in bits - but by general frames, not by spelling out a document's text.</summary>
    Coverage,

    /// <summary>The most bits taken off per step.</summary>
    Compression,

    /// <summary>The costliest unmatched text first, working down `residuals`.</summary>
    HotSpots,

    /// <summary>Generalizing the glyphs already there - merging, extracting, promoting vocabularies - alongside the text each step covers.</summary>
    Refine,
}

public static class AgentFocusExtensions
{
    /// <summary>What a session's brief tells the agent to weigh, for <paramref name="focus"/> - null for <see cref="AgentFocus.Default"/>, which adds nothing to the guide.</summary>
    public static string Brief(this AgentFocus focus) => focus switch
    {
        AgentFocus.Coverage =>
            "Coverage: weigh covering more words above taking bits off. Choose targets by the uncovered words they hold (`unmatched_openings`, `residual_phrases`), " +
            "and among the ways to cover a target, take the one that covers the most words, even where it costs bits. Still cover text with general frames - slots for what varies, " +
            "vocabularies for closed word sets - rather than literals for one document's text: spell text out literally only once nothing more general is left to find.",
        AgentFocus.Compression =>
            "Compression: weigh taking bits off above covering more words. Choose targets by the bits covering them would save - recurring text whose frame is cheap to write, " +
            "and the text of glyphs costing more than they save (`list_glyphs` with sort=net), covered more cheaply. Among the steps that pass the step rules, take the one that takes the most bits off, " +
            "and prefer reusing and widening what exists to adding a glyph.",
        AgentFocus.HotSpots =>
            "Hot spots: work down `residuals` from the costliest span. Take the top span you haven't covered, find the lines like it (`search_lines`), and cover that family - " +
            "not only the one span - then go on to the next. Pass one over only when no step can cover it, and journal it as an open problem when you do.",
        AgentFocus.Refine =>
            "Refine: weigh generalizing the grammar above reaching new text. Look for near-duplicate glyphs to merge into one with a property, repeated literal alternatives to promote into a vocabulary, " +
            "and shared sub-phrases to extract into a `[Dependent]` glyph - and make each such change in a step that also covers more words, so it passes the step rules. " +
            "A refactor that covers nothing new is the person's to approve, so save those for a check-in.",
        _ => null,
    };
}
