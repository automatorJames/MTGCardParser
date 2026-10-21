using System.Text.RegularExpressions;
using Glyphotype.Distiller.Scoring;
using Glyphotype.RegexGeneration.Debugging;

namespace Glyphotype.Distiller.Inspection;

/// <summary>Which lines a <see cref="CorpusQueries.SearchLines"/> pattern is tested against.</summary>
public enum LineScope
{
    /// <summary>The whole text of every line.</summary>
    All,

    /// <summary>Only the unmatched spans of each line - so a hit is text no glyph covers.</summary>
    Unmatched,

    /// <summary>The whole text of lines no glyph matched any of.</summary>
    Unparsed,
}

/// <summary>One distinct line, how many times it occurs, and how it tokenized.</summary>
public sealed record LineSample(string Document, string Text, string Rendering, int Occurrences);

/// <summary>What <see cref="CorpusQueries.SearchLines"/> found: counts over every hit, and the most frequent distinct lines.</summary>
public sealed record LineSearchResult(int Lines, int DistinctLines, IReadOnlyList<LineSample> Samples);

/// <summary>One shape a glyph or vocabulary was matched in - a frame (see <see cref="CorpusEncoding.GetFrame"/>), or a terminal's text - and how often.</summary>
public sealed record MatchShape(string Shape, int Occurrences, string Example, string Document);

/// <summary>Where and how one glyph or vocabulary matched across the corpus.</summary>
public sealed record MatchReport(string Name, int TopLevel, int Nested, int Documents, IReadOnlyList<MatchShape> Shapes, IReadOnlyList<LineSample> Lines);

/// <summary>A run of words recurring in unmatched text.</summary>
public sealed record ResidualPhrase(string Text, int Words, int Occurrences, int Documents);

/// <summary>Uncovered spans that open the same way, and what they come to (see <see cref="CorpusQueries.GetUnmatchedOpenings"/>).</summary>
/// <param name="Opening">The words the spans start with.</param>
/// <param name="Spans">How many uncovered spans start with them.</param>
/// <param name="Words">How many uncovered words those spans hold in all.</param>
/// <param name="Example">The commonest such span.</param>
public sealed record UnmatchedOpening(string Opening, int Spans, int Words, int Documents, string Example);

/// <summary>A line that tokenizes differently under two grammars, and how many times it occurs.</summary>
/// <param name="CapturedWordsDelta">How many more words the second grammar's matches cover, per occurrence.</param>
public sealed record LineChange(string Document, string Before, string After, int Occurrences, int CapturedWordsDelta);

/// <summary>How a corpus's tokenization changed between two grammars (see <see cref="CorpusQueries.CompareTokenizations"/>), with the most frequent changes of each kind.</summary>
/// <param name="GainedLines">Lines whose matches now cover more words.</param>
/// <param name="LostLines">Lines whose matches now cover fewer words.</param>
/// <param name="ReshapedLines">Lines covering as many words, but tokenized differently - a different glyph, or different captures.</param>
public sealed record TokenizationDiff(
    int Lines,
    int GainedLines,
    int LostLines,
    int ReshapedLines,
    IReadOnlyList<LineChange> Gained,
    IReadOnlyList<LineChange> Lost,
    IReadOnlyList<LineChange> Reshaped)
{
    public int ChangedLines => GainedLines + LostLines + ReshapedLines;
}

/// <summary>Words a change took from a glyph that matched them (see <see cref="CorpusQueries.FindSteals"/>).</summary>
/// <param name="From">The glyph that matched the words before the change and doesn't after.</param>
/// <param name="Words">How many words it lost, corpus-wide.</param>
/// <param name="Lines">How many lines it lost them on.</param>
/// <param name="TakenBy">The glyphs that match those words after the change and didn't before, with how many each took, most first - or, for words no new glyph took, whatever still matches them, or <see cref="CorpusQueries.NoGlyph"/> if nothing does.</param>
/// <param name="Examples">Lines it lost words on, as they tokenize before and after.</param>
public sealed record GlyphSteal(string From, int Words, int Lines, IReadOnlyList<(string Glyph, int Words)> TakenBy, IReadOnlyList<LineChange> Examples);

/// <summary>How far one glyph's pattern gets through a piece of text, and where it stops (see <see cref="CorpusQueries.ExplainMismatch"/>).</summary>
public sealed record MismatchExplanation(string Glyph, string Text, bool IsFullMatch, int MatchedWords, int TotalWords, string MatchedText, string FirstFailure, string Tokenization);

/// <summary>
/// Questions about a tokenized corpus, answered in plain data: which lines hold some text, how a glyph matched,
/// what recurs in the unmatched text, how two grammars' tokenizations differ, and why a glyph doesn't match
/// some text. Everything that lists lines lists distinct lines, most frequent first.
/// </summary>
public static class CorpusQueries
{
    static readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(2);

    /// <summary>How often a phrase one word longer must occur, relative to a phrase, to stand in for it (see <see cref="GetResidualPhrases"/>).</summary>
    const double _subsumingShare = 0.8;

    /// <summary>
    /// The lines matching <paramref name="pattern"/> (a case-insensitive regex; null matches every line) within
    /// <paramref name="scope"/>, and - when <paramref name="glyph"/> is given - where that glyph or vocabulary
    /// matched somewhere in the line.
    /// </summary>
    public static LineSearchResult SearchLines(IReadOnlyList<ProcessedDocument> documents, string pattern, LineScope scope = LineScope.All, string glyph = null, int limit = 20, bool nested = false)
    {
        var regex = string.IsNullOrEmpty(pattern) ? null : new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, _regexTimeout);

        var hits = Lines(documents)
            .Where(x => glyph is null || Mentions(x.Line, glyph))
            .Where(x => scope switch
            {
                LineScope.Unmatched => Unmatched(x.Line).Any(y => regex?.IsMatch(y) ?? true),
                LineScope.Unparsed => x.Line.CapturedWordCount == 0 && (regex?.IsMatch(x.Line.SourceText.FormattedText) ?? true),
                _ => regex?.IsMatch(x.Line.SourceText.FormattedText) ?? true,
            })
            .ToList();

        var distinct = Distinct(hits, nested);

        return new(hits.Count, distinct.Count, distinct.Take(limit).ToList());
    }

    /// <summary>Everywhere the glyph or vocabulary named <paramref name="name"/> matched - top-level or nested - grouped by the shape each match took.</summary>
    public static MatchReport GetMatches(IReadOnlyList<ProcessedDocument> documents, string name, int shapes = 15, int lines = 10)
    {
        var matches = Lines(documents)
            .SelectMany(x => x.Line.Glyphs
                .Where(y => !y.CaptureContext.RootCaptureTrace.IsSynthesized)
                .SelectMany(y => ParseRenderer.SelfAndDescendants(y.CaptureContext.RootCaptureTrace).Select(z => (Trace: z, IsTopLevel: z == y.CaptureContext.RootCaptureTrace)))
                .Where(y => ParseRenderer.CapturedType(y.Trace)?.Name == name)
                .Select(y => (x.Document, x.Line, y.Trace, y.IsTopLevel)))
            .ToList();

        var shapeGroups = matches
            .GroupBy(x => x.Trace.IsTerminal ? x.Trace.CaptureValue : CorpusEncoding.GetFrame(x.Trace).Text)
            .OrderByDescending(x => x.Count())
            .Take(shapes)
            .Select(x => new MatchShape(x.Key, x.Count(), x.First().Trace.IsTerminal ? null : ParseRenderer.RenderInner(x.First().Trace).Trim(), x.First().Document))
            .ToList();

        var lineSamples = Distinct(matches.Select(x => (x.Document, x.Line)).Distinct().ToList(), nested: true);

        return new(
            name,
            matches.Count(x => x.IsTopLevel),
            matches.Count(x => !x.IsTopLevel),
            matches.Select(x => x.Document).Distinct().Count(),
            shapeGroups,
            lineSamples.Take(lines).ToList());
    }

    /// <summary>
    /// Unmatched spans grouped by their first one to <paramref name="maxWords"/> words, ranked by the unmatched words
    /// the spans hold in all: where a construction's opening recurs, a glyph for it covers the most. As with
    /// <see cref="GetResidualPhrases"/>, an opening is left out when one a word longer starts nearly as many spans.
    /// </summary>
    public static IReadOnlyList<UnmatchedOpening> GetUnmatchedOpenings(IReadOnlyList<ProcessedDocument> documents, int maxWords = 3, int minOccurrences = 3, int limit = 25)
    {
        var spans = Lines(documents)
            .SelectMany(x => Unmatched(x.Line).Select(y => (x.Document, Text: y, Words: y.Split(' ', StringSplitOptions.RemoveEmptyEntries))))
            .Where(x => x.Words.Length > 0)
            .ToList();

        var groups = new Dictionary<string, List<(string Document, string Text, string[] Words)>>();

        foreach (var span in spans)
            for (int n = 1; n <= Math.Min(maxWords, span.Words.Length); n++)
            {
                var opening = string.Join(' ', span.Words, 0, n);

                if (!groups.TryGetValue(opening, out var group))
                    groups[opening] = group = [];

                group.Add(span);
            }

        // An opening one word longer, starting nearly as many spans, makes this one redundant.
        var subsumed = groups
            .Where(x => x.Key.Contains(' '))
            .Where(x => x.Value.Count >= _subsumingShare * groups[x.Key[..x.Key.LastIndexOf(' ')]].Count)
            .Select(x => x.Key[..x.Key.LastIndexOf(' ')])
            .ToHashSet();

        return groups
            .Where(x => x.Value.Count >= minOccurrences && !subsumed.Contains(x.Key))
            .Select(x => new UnmatchedOpening(
                x.Key,
                x.Value.Count,
                x.Value.Sum(y => y.Words.Length),
                x.Value.Select(y => y.Document).Distinct().Count(),
                x.Value.GroupBy(y => y.Text).OrderByDescending(y => y.Count()).ThenBy(y => y.Key, StringComparer.Ordinal).First().Key))
            .OrderByDescending(x => x.Words)
            .ThenBy(x => x.Opening, StringComparer.Ordinal)
            .Take(limit)
            .ToList();
    }

    /// <summary>
    /// Runs of <paramref name="minWords"/> to <paramref name="maxWords"/> words that recur in unmatched text at least
    /// <paramref name="minOccurrences"/> times, never crossing from one unmatched span into the next - ranked by the
    /// words they'd account for. A phrase is left out when one a word longer occurs nearly as often (at least
    /// <see cref="_subsumingShare"/> as often) - so each repetition is listed once, in its longest common form, rather
    /// than as every overlapping piece of it.
    /// </summary>
    public static IReadOnlyList<ResidualPhrase> GetResidualPhrases(IReadOnlyList<ProcessedDocument> documents, int minWords = 2, int maxWords = 8, int minOccurrences = 3, int limit = 40)
    {
        var counts = new Dictionary<string, (int Occurrences, HashSet<string> Documents)>();

        foreach (var (document, line) in Lines(documents))
            foreach (var span in Unmatched(line))
            {
                var words = span.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                for (int n = minWords; n <= Math.Min(maxWords + 1, words.Length); n++)
                    for (int i = 0; i + n <= words.Length; i++)
                    {
                        var phrase = string.Join(' ', words, i, n);

                        if (!counts.TryGetValue(phrase, out var entry))
                            counts[phrase] = entry = (0, []);

                        entry.Documents.Add(document);
                        counts[phrase] = (entry.Occurrences + 1, entry.Documents);
                    }
            }

        // A phrase one word longer on either side, occurring nearly as often, makes this one redundant.
        var subsumed = new HashSet<string>();

        foreach (var (phrase, entry) in counts)
        {
            var space = phrase.IndexOf(' ');
            var lastSpace = phrase.LastIndexOf(' ');

            if (space < 0)
                continue;

            foreach (var shorter in new[] { phrase[(space + 1)..], phrase[..lastSpace] })
                if (counts.TryGetValue(shorter, out var shorterEntry) && entry.Occurrences >= _subsumingShare * shorterEntry.Occurrences)
                    subsumed.Add(shorter);
        }

        return counts
            .Where(x => x.Value.Occurrences >= minOccurrences && !subsumed.Contains(x.Key))
            .Select(x => new ResidualPhrase(x.Key, x.Key.Count(c => c == ' ') + 1, x.Value.Occurrences, x.Value.Documents.Count))
            .Where(x => x.Words >= minWords && x.Words <= maxWords)
            .OrderByDescending(x => (long)x.Words * x.Occurrences)
            .ThenBy(x => x.Text, StringComparer.Ordinal)
            .Take(limit)
            .ToList();
    }

    /// <summary>
    /// How the tokenization of the same documents differs between <paramref name="before"/> and <paramref name="after"/>
    /// (each tokenized by a different grammar, in the same order): line by line, as distinct changes, most frequent first.
    /// </summary>
    public static TokenizationDiff CompareTokenizations(IReadOnlyList<ProcessedDocument> before, IReadOnlyList<ProcessedDocument> after, int limit = 10)
    {
        if (before.Count != after.Count)
            throw new ArgumentException("Both tokenizations must be of the same documents, in the same order");

        List<(string Document, ProcessedLine Before, ProcessedLine After)> changed = [];
        var lines = 0;

        for (int i = 0; i < before.Count; i++)
            foreach (var (beforeLine, afterLine) in before[i].Lines.Zip(after[i].Lines))
            {
                lines++;

                // An incremental re-tokenization keeps the very line it didn't tokenize again (see ProcessedDocument.Retokenize).
                if (ReferenceEquals(beforeLine, afterLine))
                    continue;

                if (ParseRenderer.Render(beforeLine, nested: true) != ParseRenderer.Render(afterLine, nested: true))
                    changed.Add((before[i].Document.Name, beforeLine, afterLine));
            }

        var changes = changed
            .GroupBy(x => (Before: ParseRenderer.Render(x.Before, nested: false), After: ParseRenderer.Render(x.After, nested: false), Delta: x.After.CapturedWordCount - x.Before.CapturedWordCount))
            .Select(x => new LineChange(x.First().Document, x.Key.Before, x.Key.After, x.Count(), x.Key.Delta))
            .OrderByDescending(x => x.Occurrences * Math.Max(1, Math.Abs(x.CapturedWordsDelta)))
            .ToList();

        var gained = changes.Where(x => x.CapturedWordsDelta > 0).ToList();
        var lost = changes.Where(x => x.CapturedWordsDelta < 0).ToList();
        var reshaped = changes.Where(x => x.CapturedWordsDelta == 0).ToList();

        return new(
            lines,
            gained.Sum(x => x.Occurrences),
            lost.Sum(x => x.Occurrences),
            reshaped.Sum(x => x.Occurrences),
            gained.Take(limit).ToList(),
            lost.Take(limit).ToList(),
            reshaped.Take(limit).ToList());
    }

    /// <summary>What <see cref="GlyphSteal.TakenBy"/> lists for words no glyph matches after the change.</summary>
    public const string NoGlyph = "(unmatched)";

    /// <summary>
    /// The words a change took from the glyphs that matched them: a word belongs to every glyph whose match - top-level
    /// or nested, through a dynamic too - spans it, and a glyph loses it when it belongs to that glyph in
    /// <paramref name="before"/> but not in <paramref name="after"/> (each tokenized by a different grammar, in the same
    /// order). Only glyphs named in <paramref name="glyphNames"/> count, so a primitive (a list, a one-of) owns nothing.
    /// Most words first.
    /// </summary>
    public static IReadOnlyList<GlyphSteal> FindSteals(IReadOnlyList<ProcessedDocument> before, IReadOnlyList<ProcessedDocument> after, IReadOnlySet<string> glyphNames, int examples = 2)
    {
        if (before.Count != after.Count)
            throw new ArgumentException("Both tokenizations must be of the same documents, in the same order");

        var steals = new Dictionary<string, (int Words, Dictionary<string, int> TakenBy, List<(string Document, ProcessedLine Before, ProcessedLine After)> Lines)>();

        for (int i = 0; i < before.Count; i++)
            foreach (var (beforeLine, afterLine) in before[i].Lines.Zip(after[i].Lines))
            {
                if (ReferenceEquals(beforeLine, afterLine) || ParseRenderer.Render(beforeLine, nested: true) == ParseRenderer.Render(afterLine, nested: true))
                    continue;

                var (was, now) = (WordOwners(beforeLine, glyphNames), WordOwners(afterLine, glyphNames));

                for (int word = 0; word < Math.Min(was.Count, now.Count); word++)
                    foreach (var from in was[word].Except(now[word]))
                    {
                        if (!steals.TryGetValue(from, out var steal))
                            steals[from] = steal = (0, [], []);

                        steal.Words++;
                        steals[from] = steal;

                        List<string> takers = [.. now[word].Except(was[word])];

                        if (takers.Count == 0)
                            takers = now[word].Count == 0 ? [NoGlyph] : [.. now[word]];

                        foreach (var taker in takers)
                            steal.TakenBy[taker] = steal.TakenBy.GetValueOrDefault(taker) + 1;

                        if (steal.Lines.Count == 0 || steal.Lines[^1].Before != beforeLine)
                            steal.Lines.Add((before[i].Document.Name, beforeLine, afterLine));
                    }
            }

        return steals
            .Select(x => new GlyphSteal(
                x.Key,
                x.Value.Words,
                x.Value.Lines.Count,
                x.Value.TakenBy.OrderByDescending(y => y.Value).ThenBy(y => y.Key, StringComparer.Ordinal).Select(y => (y.Key, y.Value)).ToList(),
                x.Value.Lines
                    .GroupBy(y => (Before: ParseRenderer.Render(y.Before, nested: false), After: ParseRenderer.Render(y.After, nested: false), Delta: y.After.CapturedWordCount - y.Before.CapturedWordCount))
                    .Select(y => new LineChange(y.First().Document, y.Key.Before, y.Key.After, y.Count(), y.Key.Delta))
                    .OrderByDescending(y => y.Occurrences)
                    .Take(examples)
                    .ToList()))
            .OrderByDescending(x => x.Words)
            .ThenBy(x => x.From, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>For each word of <paramref name="line"/>, in order, the glyphs named in <paramref name="glyphNames"/> whose matches span it.</summary>
    static List<HashSet<string>> WordOwners(ProcessedLine line, IReadOnlySet<string> glyphNames)
    {
        if (line.Glyphs.FirstOrDefault()?.CaptureContext.SourceText is not string text)
            return [];

        var words = Regex.Matches(text, @"\S+").Select(x => (Start: x.Index, End: x.Index + x.Length)).ToList();
        var owners = words.Select(_ => new HashSet<string>()).ToList();

        foreach (var trace in line.Glyphs.Where(x => !x.CaptureContext.RootCaptureTrace.IsSynthesized).SelectMany(x => ParseRenderer.SelfAndDescendants(x.CaptureContext.RootCaptureTrace)))
        {
            if (ParseRenderer.CapturedType(trace)?.Name is not string name || !glyphNames.Contains(name))
                continue;

            for (int word = 0; word < words.Count; word++)
                if (words[word].Start < trace.Index + trace.Length && trace.Index < words[word].End)
                    owners[word].Add(name);
        }

        return owners;
    }

    /// <summary><paramref name="text"/> (lower-cased, as corpus lines are) tokenized by <paramref name="grammar"/>, line by line.</summary>
    public static IReadOnlyList<string> Tokenize(GlyphGrammar grammar, string text, bool nested = true) =>
        text.ToLowerInvariant().Split('\n').Select(x => x.Trim()).Where(x => x.Length > 0)
            .Select(x => ParseRenderer.Render(grammar.Tokenize(x), nested))
            .ToList();

    /// <summary>
    /// How much of <paramref name="text"/> (lower-cased, as corpus lines are) the glyph named <paramref name="glyph"/>
    /// matches on its own, from the start, and the first part of its pattern that fails - alongside how the whole
    /// grammar actually tokenizes the text, since a glyph that matches can still lose to another.
    /// </summary>
    public static MismatchExplanation ExplainMismatch(GlyphGrammar grammar, string glyph, string text)
    {
        if (!grammar.TryGetType(glyph, out var type))
            throw new ArgumentException($"There's no glyph named {glyph} in this grammar");

        text = text.ToLowerInvariant().Trim();
        var result = RegexMatchDebugger.Analyze(GlyphTypeCache.GetRegexGraph(type), text, grammar);

        return new(
            glyph,
            text,
            result.IsFullMatch,
            result.MatchedWordCount,
            result.TotalWordCount,
            text[..Math.Min(result.MatchedCharCount, text.Length)],
            result.IsFullMatch ? null : result.FirstFailureDisplay,
            ParseRenderer.Render(grammar.Tokenize(text), nested: true));
    }

    static IEnumerable<(string Document, ProcessedLine Line)> Lines(IReadOnlyList<ProcessedDocument> documents) =>
        documents.SelectMany(x => x.Lines.Select(y => (x.Document.Name, y)));

    /// <summary>A line's unmatched spans.</summary>
    static IEnumerable<string> Unmatched(ProcessedLine line) =>
        line.Glyphs
            .Where(x => x.CaptureContext.RootCaptureTrace.IsUnmatchedString)
            .Select(x => x.CaptureValue.Trim());

    static bool Mentions(ProcessedLine line, string name) =>
        line.Glyphs
            .Where(x => !x.CaptureContext.RootCaptureTrace.IsSynthesized)
            .SelectMany(x => ParseRenderer.SelfAndDescendants(x.CaptureContext.RootCaptureTrace))
            .Any(x => ParseRenderer.CapturedType(x)?.Name == name);

    static List<LineSample> Distinct(List<(string Document, ProcessedLine Line)> lines, bool nested) =>
        lines
            .GroupBy(x => x.Line.SourceText.FormattedText)
            .OrderByDescending(x => x.Count())
            .Select(x => new LineSample(x.First().Document, x.Key, ParseRenderer.Render(x.First().Line, nested), x.Count()))
            .ToList();
}
