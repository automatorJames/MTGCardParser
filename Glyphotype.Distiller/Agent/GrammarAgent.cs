using System.Text.RegularExpressions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Scoring;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Distiller.Agent;

/// <summary>A request an agent made that can't be carried out, with a message written for the agent to act on.</summary>
public sealed class AgentRequestException(string message, Exception inner = null) : Exception(message, inner);

/// <summary>
/// Everything an agent needs to compose a grammar over one <see cref="GrammarWorkbench"/>, as plain-text reports:
/// orientation (score, glyphs, definitions), inspection (lines, matches, residual text), probing (tokenize and
/// explain a draft on any text), and stepping (evaluate a change set without making it, apply it as a step,
/// undo). The workbench is shared - a person editing the same one sees every step, and the agent sees theirs.
/// <para>
/// Host-independent: a tool server (or anything else) exposes these methods as they are. Every method reads the
/// workbench's state afresh, and waits for a re-score under way. Requests that can't be carried out throw
/// <see cref="AgentRequestException"/>, with a message saying what to do instead.
/// </para>
/// </summary>
public sealed class GrammarAgent(GrammarWorkbench workbench, string corpusDescription)
{
    /// <summary>The guide to the workbench, its scoring, and writing glyphs - see <c>AgentGuide.md</c>.</summary>
    public static string Guide { get; } = LoadGuide();

    /// <summary>A short description of the tools and where to start, for a tool server's instructions.</summary>
    public const string Instructions =
        "Tools for composing a grammar over a text corpus, glyph by glyph, on a working definition shared live with a person. " +
        "Start with `overview`, then read `guide` once: it explains the loop (find recurring unmatched text, draft a glyph in C#, " +
        "`tokenize`/`explain_mismatch` it, `evaluate` it, `apply` it), how the score works, and how to write glyphs. " +
        "Committing to C# source is the person's decision, made in the app - never part of the loop.";

    static string LoadGuide()
    {
        using var stream = typeof(GrammarAgent).Assembly.GetManifestResourceStream("AgentGuide.md")
            ?? throw new InvalidOperationException("The agent guide isn't embedded in the assembly");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    // ---- Orientation ----

    public async Task<string> OverviewAsync(CancellationToken cancellation = default)
    {
        var trial = await workbench.GetCurrentTrialAsync(cancellation);
        var definition = trial.Definition;
        var report = new StringBuilder();

        report.AppendLine($"Corpus: {corpusDescription} ({workbench.Documents.Count:N0} documents).");
        report.AppendLine($"Working grammar: {definition.Glyphs.Count} glyphs, {definition.Vocabularies.Count} vocabularies. "
            + $"{workbench.Changes.Count} uncommitted change{S(workbench.Changes.Count)}, {workbench.History.Count} step{S(workbench.History.Count)} this session.");

        if (!trial.Succeeded)
        {
            report.AppendLine();
            report.AppendLine("The working definition doesn't build, so nothing can be scored or inspected until it's fixed (or the breaking step undone):");
            AppendErrors(report, trial.Errors);
            return report.ToString();
        }

        var score = trial.Score;
        report.AppendLine($"Score: {score.TotalBits:N0} bits = {score.CompressionRatio:P2} of the no-grammar baseline ({score.BaselineBits:N0}). Lower is better.");
        report.AppendLine($"  grammar {score.GrammarBits:N0} · {string.Join(" · ", score.ComponentBits.Select(x => $"{x.Key.ToString().ToLowerInvariant()} {x.Value:N0}"))}");
        report.AppendLine($"Coverage: {score.Coverage:P2} of words inside matches ({score.CapturedWords:N0} of {score.Words:N0}, {score.Lines:N0} lines).");

        var losing = score.Glyphs.Where(x => x.IsTopLevel && x.NetBits < 0).OrderBy(x => x.NetBits).ToList();

        if (losing.Count > 0)
            report.AppendLine($"Top-level glyphs costing more than they save: {string.Join(", ", losing.Take(8).Select(x => $"{x.Name} ({x.NetBits:N0})"))}{(losing.Count > 8 ? $", … {losing.Count - 8} more" : "")}.");

        report.AppendLine();
        report.AppendLine("Costliest unmatched text:");
        AppendResiduals(report, score.Residuals.OrderByDescending(x => x.Bits).Take(10), score);

        if (workbench.Changes.Count == 0 && workbench.History.Count == 0)
        {
            report.AppendLine();
            report.AppendLine("New here? Read `guide` before your first change.");
        }

        return report.ToString();
    }

    /// <summary>Every glyph with its contribution, and every vocabulary with its cost and users.</summary>
    /// <param name="sort">"net" (default), "matches", "name" or "cost".</param>
    public async Task<string> ListGlyphsAsync(string sort = "net", CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);
        var definition = trial.Definition;
        var contributions = trial.Score.Glyphs.ToDictionary(x => x.Name);
        var status = workbench.Changes.ToDictionary(x => (x.Kind, x.Name), x => x.Change);

        var glyphs = definition.Glyphs.Select(x => (Glyph: x, Contribution: contributions.GetValueOrDefault(x.Name))).ToList();

        glyphs = (sort?.ToLowerInvariant()) switch
        {
            "name" => glyphs.OrderBy(x => x.Glyph.Name, StringComparer.Ordinal).ToList(),
            "matches" => glyphs.OrderByDescending(x => x.Contribution?.Occurrences ?? 0).ToList(),
            "cost" => glyphs.OrderByDescending(x => x.Contribution?.DefinitionBits ?? 0).ToList(),
            _ => glyphs.OrderByDescending(x => x.Contribution?.NetBits ?? 0).ToList(),
        };

        var report = new StringBuilder();
        report.AppendLine($"Glyphs ({glyphs.Count}; {glyphs.Count(x => x.Contribution?.IsTopLevel == true)} top-level). Matches and words count top-level matches; nested-only glyphs are credited to the glyphs using them.");
        report.AppendLine($"  {"net",9} {"matches",8} {"words",7} {"def",6}  glyph");

        foreach (var (glyph, contribution) in glyphs)
        {
            List<string> notes = [];

            if (glyph.Kind != GlyphKind.Glyph)
                notes.Add(glyph.Kind == GlyphKind.Alias ? $"alias of {glyph.AliasOf}" : "one-of");

            if (contribution?.IsTopLevel != true)
                notes.Add(glyph.IsDependent ? "dependent" : "nested only");

            if (status.TryGetValue((DefinitionKind.Glyph, glyph.Name), out var change))
                notes.Add(change.ToString().ToLowerInvariant() + " since commit");

            report.AppendLine($"  {contribution?.NetBits ?? 0,9:N0} {contribution?.Occurrences ?? 0,8:N0} {contribution?.Words ?? 0,7:N0} {contribution?.DefinitionBits ?? 0,6:N0}  {glyph.Name}{(notes.Count > 0 ? $"  ({string.Join(", ", notes)})" : "")}");
        }

        report.AppendLine();
        report.AppendLine($"Vocabularies ({definition.Vocabularies.Count}):");
        report.AppendLine($"  {"cost",7} {"members",8}  vocabulary  (used by)");

        foreach (var vocabulary in definition.Vocabularies.OrderByDescending(x => trial.Score.Vocabularies.GetValueOrDefault(x.Name)))
        {
            var users = definition.GetReferrers(vocabulary.Name);
            var changed = status.TryGetValue((DefinitionKind.Vocabulary, vocabulary.Name), out var change) ? $", {change.ToString().ToLowerInvariant()} since commit" : "";

            report.AppendLine($"  {trial.Score.Vocabularies.GetValueOrDefault(vocabulary.Name),7:N0} {vocabulary.Members.Count,8}  {vocabulary.Name}  ({(users.Count > 0 ? string.Join(", ", users) : "unused")}{changed})");
        }

        return report.ToString();
    }

    /// <summary>The C# source of the named glyphs, vocabularies and markers in the working definition.</summary>
    /// <param name="names">Comma- or space-separated names.</param>
    public string Show(string names)
    {
        var definition = workbench.WorkingDefinition;
        var status = workbench.Changes.ToDictionary(x => x.Name, x => x.Change);
        var report = new StringBuilder();

        foreach (var name in SplitNames(names))
        {
            var note = status.TryGetValue(name, out var change) ? $"// {change.ToString().ToLowerInvariant()} since the last commit{Environment.NewLine}" : "";

            if (definition.Glyphs.FirstOrDefault(x => x.Name == name) is GlyphDefinition glyph)
                report.Append(note).AppendLine(GlyphSourceWriter.WriteGlyph(glyph));
            else if (definition.Vocabularies.FirstOrDefault(x => x.Name == name) is VocabularyDefinition vocabulary)
                report.Append(note).AppendLine(GlyphSourceWriter.WriteVocabulary(vocabulary));
            else if (definition.Markers.Contains(name))
                report.Append(note).AppendLine(GlyphSourceWriter.WriteMarker(name));
            else
                report.AppendLine($"// {name}: no glyph, vocabulary or marker has this name{Suggest(name, definition)}").AppendLine();
        }

        return report.ToString().TrimEnd() + Environment.NewLine;
    }

    // ---- Stepping ----

    /// <summary>What applying a change set would do - to the score, and to how lines tokenize - without applying it.</summary>
    /// <param name="source">C# declarations to add or replace (see the guide).</param>
    /// <param name="remove">Comma- or space-separated names of definitions to remove.</param>
    public async Task<string> EvaluateAsync(string source, string remove = null, CancellationToken cancellation = default)
    {
        var changes = ReadChanges(source, remove);
        var evaluation = await Evaluate(changes, cancellation);

        return $"Evaluated, not applied: {changes.Describe()}{Environment.NewLine}{DescribeEvaluation(evaluation)}"
            + (evaluation.After.Succeeded ? $"{Environment.NewLine}To make this change, `apply` the same source and removals." : "");
    }

    /// <summary>Makes a change set as one step of the working definition - refused if the result wouldn't build.</summary>
    /// <param name="description">Why: shown to the person in the step history.</param>
    public async Task<string> ApplyAsync(string source, string remove = null, string description = null, CancellationToken cancellation = default)
    {
        var changes = ReadChanges(source, remove);
        var evaluation = await Evaluate(changes, cancellation);

        if (!evaluation.After.Succeeded)
            return $"Not applied - the result wouldn't build: {changes.Describe()}{Environment.NewLine}{DescribeEvaluation(evaluation)}";

        var step = Try(() => workbench.Apply(changes, description))
            ?? throw new AgentRequestException("Nothing to apply: the working definition already reads exactly like this.");

        return $"Applied as step {step.Number}: {step.Description}{Environment.NewLine}{DescribeEvaluation(evaluation)}";
    }

    /// <summary>Takes back the latest step, whoever made it.</summary>
    public async Task<string> UndoAsync(CancellationToken cancellation = default)
    {
        var before = await workbench.GetCurrentTrialAsync(cancellation);
        var step = Try(workbench.Undo);
        var after = await workbench.GetCurrentTrialAsync(cancellation);

        return $"Undid step {step.Number}: {step.Description}{Environment.NewLine}{DescribeEvaluation(new(new ChangeSet(), before, after), lineLimit: 5)}";
    }

    /// <summary>Returns the named glyph or vocabulary to its committed state, as one step.</summary>
    public async Task<string> RevertAsync(string name, CancellationToken cancellation = default)
    {
        var before = await workbench.GetCurrentTrialAsync(cancellation);
        var kind = workbench.Changes.FirstOrDefault(x => x.Name == name)?.Kind
            ?? throw new AgentRequestException($"{name} has no uncommitted changes to revert{Suggest(name, workbench.WorkingDefinition)}");

        Try(() => workbench.Revert(kind, name));
        var after = await workbench.GetCurrentTrialAsync(cancellation);

        return $"Reverted {kind.ToString().ToLowerInvariant()} {name} to its committed state{Environment.NewLine}{DescribeEvaluation(new(new ChangeSet(), before, after), lineLimit: 5)}";
    }

    /// <summary>The steps made this session, newest first, and what's uncommitted.</summary>
    public string History(int limit = 20)
    {
        var history = workbench.History;
        var report = new StringBuilder();

        report.AppendLine(history.Count == 0
            ? "No steps this session."
            : $"Steps this session, newest first ({history.Count}; `undo` takes back the top one):");

        foreach (var step in history.AsEnumerable().Reverse().Take(limit))
            report.AppendLine($"  #{step.Number,-4} {step.At:HH:mm:ss}  {step.Description}");

        if (history.Count > limit)
            report.AppendLine($"  … {history.Count - limit} older");

        report.AppendLine();
        report.AppendLine(workbench.Changes.Count == 0
            ? "The working definition matches the committed grammar."
            : $"Uncommitted changes against the committed grammar: {string.Join(", ", workbench.Changes.Select(x => $"{x.Name} ({x.Change.ToString().ToLowerInvariant()})"))}");

        return report.ToString();
    }

    // ---- Inspection ----

    /// <summary>Distinct lines matching a regex, most frequent first, as the working grammar tokenizes them.</summary>
    public async Task<string> SearchLinesAsync(string pattern, LineScope scope = LineScope.All, string glyph = null, int limit = 20, bool nested = false, CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);
        var result = Try(() => CorpusQueries.SearchLines(trial.Documents, pattern, scope, NullIfBlank(glyph), limit, nested));

        var report = new StringBuilder();
        report.AppendLine($"{result.Lines:N0} line{S(result.Lines)} ({result.DistinctLines:N0} distinct){(result.DistinctLines > result.Samples.Count ? $", showing the {result.Samples.Count} most frequent" : "")}:");
        AppendLines(report, result.Samples);
        return report.ToString();
    }

    /// <summary>Where and how a glyph or vocabulary matched: counts, the shapes its matches took, and example lines.</summary>
    public async Task<string> MatchesAsync(string name, int shapes = 15, int lines = 8, CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);

        if (!trial.Definition.Glyphs.Any(x => x.Name == name) && !trial.Definition.Vocabularies.Any(x => x.Name == name))
            throw new AgentRequestException($"There's no glyph or vocabulary named {name}{Suggest(name, trial.Definition)}");

        var matches = CorpusQueries.GetMatches(trial.Documents, name, shapes, lines);
        var report = new StringBuilder();

        report.AppendLine($"{name}: {matches.TopLevel:N0} top-level and {matches.Nested:N0} nested match{(matches.Nested == 1 ? "" : "es")}, in {matches.Documents:N0} document{S(matches.Documents)}.");

        if (matches.Shapes.Count > 0)
        {
            report.AppendLine("Shapes (frames with captures masked, or a terminal's text), most frequent first:");

            foreach (var shape in matches.Shapes)
                report.AppendLine($"  ×{shape.Occurrences,-5} {shape.Shape}{(shape.Example is null ? "" : $"      e.g. {shape.Example}")}");

            report.AppendLine("Lines:");
            AppendLines(report, matches.Lines);
        }

        return report.ToString();
    }

    /// <summary>The costliest distinct spans of unmatched text.</summary>
    public async Task<string> ResidualsAsync(int limit = 30, int offset = 0, int minWords = 1, CancellationToken cancellation = default)
    {
        var score = (await GetScoredTrialAsync(cancellation)).Score;
        var residuals = score.Residuals.Where(x => x.Words >= minWords).OrderByDescending(x => x.Bits).ToList();

        var report = new StringBuilder();
        report.AppendLine($"{residuals.Count:N0} distinct unmatched spans ({score.ComponentBits[DataComponent.Residual]:N0} bits in all), costliest first{(offset > 0 ? $", from #{offset + 1}" : "")}:");
        AppendResiduals(report, residuals.Skip(offset).Take(limit), score);
        return report.ToString();
    }

    /// <summary>Word runs that recur in unmatched text - the raw material for new glyphs.</summary>
    public async Task<string> ResidualPhrasesAsync(int minWords = 2, int maxWords = 8, int minOccurrences = 3, int limit = 40, CancellationToken cancellation = default)
    {
        var trial = await GetScoredTrialAsync(cancellation);
        var phrases = CorpusQueries.GetResidualPhrases(trial.Documents, minWords, maxWords, minOccurrences, limit);

        var report = new StringBuilder();
        report.AppendLine($"Recurring phrases in unmatched text ({minWords}-{maxWords} words, at least {minOccurrences}×), by words they'd account for:");
        report.AppendLine($"  {"count",6} {"docs",5}  phrase");

        foreach (var phrase in phrases)
            report.AppendLine($"  {phrase.Occurrences,6:N0} {phrase.Documents,5:N0}  {phrase.Text}");

        if (phrases.Count == 0)
            report.AppendLine("  (none)");

        return report.ToString();
    }

    // ---- Probing ----

    /// <summary>How text tokenizes - under the working grammar, or with a draft change set applied on top of it.</summary>
    public async Task<string> TokenizeAsync(string text, string source = null, string remove = null, CancellationToken cancellation = default)
    {
        var grammar = await GetGrammarAsync(source, remove, cancellation);
        return string.Join(Environment.NewLine, CorpusQueries.Tokenize(grammar, text));
    }

    /// <summary>How far one glyph's pattern gets through a piece of text on its own, and which part stops it.</summary>
    public async Task<string> ExplainMismatchAsync(string glyph, string text, string source = null, string remove = null, CancellationToken cancellation = default)
    {
        var grammar = await GetGrammarAsync(source, remove, cancellation);
        var explanation = Try(() => CorpusQueries.ExplainMismatch(grammar, glyph, text));

        var report = new StringBuilder();

        if (explanation.IsFullMatch)
        {
            report.AppendLine($"{glyph}'s pattern matches {(explanation.MatchedWords == explanation.TotalWords ? "all of the text" : $"the first {explanation.MatchedWords} of {explanation.TotalWords} words: \"{explanation.MatchedText}\"")}.");

            if (explanation.MatchedWords < explanation.TotalWords)
                report.AppendLine("A top-level glyph must match a whole clause, so unless it's nested (or allows partial matches), the rest of the clause stops it.");
        }
        else
        {
            report.AppendLine($"{glyph}'s pattern matches {explanation.MatchedWords} of {explanation.TotalWords} words, then fails.");
            report.AppendLine($"  matched: \"{explanation.MatchedText}\"");
            report.AppendLine($"  first failing part: {explanation.FirstFailure}");
        }

        report.AppendLine($"The whole grammar tokenizes it as: {explanation.Tokenization}");
        return report.ToString();
    }

    // ---- Helpers ----

    ChangeSet ReadChanges(string source, string remove)
    {
        var changes = Try(() => ChangeSet.FromSource(source, SplitNames(remove), workbench.WorkingDefinition));

        if (changes.IsEmpty)
            throw new AgentRequestException("The change set is empty: pass C# declarations as source, names to remove, or both.");

        return changes;
    }

    async Task<Evaluation> Evaluate(ChangeSet changes, CancellationToken cancellation)
    {
        var evaluation = await TryAsync(() => workbench.EvaluateAsync(changes, cancellation));

        if (!evaluation.Before.Succeeded)
            throw new AgentRequestException("The working definition doesn't build, so there's nothing to compare against. Fix it or undo the breaking step first:"
                + Environment.NewLine + string.Join(Environment.NewLine, evaluation.Before.Errors.Select(x => "  " + x)));

        return evaluation;
    }

    static string DescribeEvaluation(Evaluation evaluation, int lineLimit = 8)
    {
        var report = new StringBuilder();

        if (!evaluation.After.Succeeded)
        {
            report.AppendLine("The grammar doesn't build:");
            AppendErrors(report, evaluation.After.Errors);
            return report.ToString();
        }

        if (!evaluation.Before.Succeeded)
        {
            report.AppendLine($"Now builds again: {evaluation.After.Score.TotalBits:N0} bits, coverage {evaluation.After.Score.Coverage:P2}.");
            return report.ToString();
        }

        report.Append(new ScoreComparison(evaluation.Before.Score, evaluation.After.Score).ToReport());

        var diff = CorpusQueries.CompareTokenizations(evaluation.Before.Documents, evaluation.After.Documents, lineLimit);

        report.AppendLine($"Lines: {diff.GainedLines:N0} gained, {diff.LostLines:N0} lost, {diff.ReshapedLines:N0} reshaped, of {diff.Lines:N0}.");
        AppendChanges(report, "Gained (more words covered)", diff.Gained, diff.GainedLines);
        AppendChanges(report, "Lost (fewer words covered)", diff.Lost, diff.LostLines);
        AppendChanges(report, "Reshaped (same coverage, different parse)", diff.Reshaped, diff.ReshapedLines);

        return report.ToString();
    }

    static void AppendChanges(StringBuilder report, string heading, IReadOnlyList<LineChange> changes, int total)
    {
        if (changes.Count == 0)
            return;

        report.AppendLine($"{heading}:");

        foreach (var change in changes)
        {
            report.AppendLine($"  ×{change.Occurrences,-4} {change.Before}");
            report.AppendLine($"     → {change.After}{(change.CapturedWordsDelta != 0 ? $"   ({(change.CapturedWordsDelta > 0 ? "+" : "")}{change.CapturedWordsDelta} words)" : "")}");
        }

        var shown = changes.Sum(x => x.Occurrences);

        if (total > shown)
            report.AppendLine($"  … {total - shown:N0} more line{S(total - shown)}");
    }

    static void AppendLines(StringBuilder report, IEnumerable<LineSample> lines)
    {
        foreach (var line in lines)
            report.AppendLine($"  ×{line.Occurrences,-4} {line.Rendering}   [{line.Document}]");
    }

    static void AppendResiduals(StringBuilder report, IEnumerable<ResidualSpan> residuals, MdlScore score)
    {
        report.AppendLine($"  {"bits",8} {"count",6} {"words",6}  text");

        foreach (var span in residuals)
            report.AppendLine($"  {span.Bits,8:N0} {span.Occurrences,6:N0} {span.Words,6:N0}  {Truncate(span.Text, 160)}");
    }

    static void AppendErrors(StringBuilder report, IEnumerable<string> errors)
    {
        foreach (var error in errors)
            report.AppendLine("  " + error);
    }

    async Task<WorkingScore> GetScoredTrialAsync(CancellationToken cancellation)
    {
        var trial = await workbench.GetCurrentTrialAsync(cancellation);

        if (!trial.Succeeded)
            throw new AgentRequestException("The working definition doesn't build, so the corpus can't be inspected. Fix it (`apply`) or `undo` the breaking step:"
                + Environment.NewLine + string.Join(Environment.NewLine, trial.Errors.Select(x => "  " + x)));

        return trial;
    }

    async Task<GlyphGrammar> GetGrammarAsync(string source, string remove, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(remove))
            return (await GetScoredTrialAsync(cancellation)).Grammar;

        var definition = Try(() => ReadChanges(source, remove).ApplyTo(workbench.WorkingDefinition));

        try
        {
            return workbench.BuildGrammar(definition);
        }
        catch (AggregateException exception)
        {
            throw new AgentRequestException("The draft doesn't build:" + Environment.NewLine
                + string.Join(Environment.NewLine, exception.Message.Split('\n').Select(x => x.Trim()).Where(x => x.Length > 0).Select(x => "  " + x)), exception);
        }
    }

    /// <summary>Runs <paramref name="action"/>, turning the exceptions a bad request causes into <see cref="AgentRequestException"/>s.</summary>
    static T Try<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception exception) when (IsBadRequest(exception))
        {
            throw new AgentRequestException(exception.Message, exception);
        }
    }

    static void Try(Action action) => Try(() => { action(); return 0; });

    static async Task<T> TryAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception exception) when (IsBadRequest(exception))
        {
            throw new AgentRequestException(exception.Message, exception);
        }
    }

    static bool IsBadRequest(Exception exception) =>
        exception is GlyphSourceException or InvalidOperationException or ArgumentException or RegexMatchTimeoutException;

    static IEnumerable<string> SplitNames(string names) =>
        (names ?? "").Split([',', ' ', '\n', '\r', '\t', ';'], StringSplitOptions.RemoveEmptyEntries).Distinct();

    static string Suggest(string name, GrammarDefinition definition)
    {
        var similar = definition.Glyphs.Select(x => x.Name).Concat(definition.Vocabularies.Select(x => x.Name))
            .Where(x => x.Contains(name, StringComparison.OrdinalIgnoreCase) || name.Contains(x, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .ToList();

        return similar.Count > 0 ? $" - did you mean {string.Join(", ", similar)}?" : "";
    }

    static string NullIfBlank(string text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    static string S(int count) => count == 1 ? "" : "s";

    static string Truncate(string text, int length) => text.Length <= length ? text : text[..(length - 1)] + "…";
}
