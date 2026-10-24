namespace Glyphotype.GlyphAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Represents a single, fully processed line from a document, containing both the
/// hierarchical analysis of matched tokens (GlyphCaptureSummaries) and a list of any
/// unmatched occurrences.
/// </summary>
public class ProcessedLine
{
    public SourceTextDTO SourceText { get; set; }

    /// <summary>
    /// The hierarchical representation of matched tokens on this line. Each contains a root CaptureTrace.
    /// </summary>
    public List<CaptureUnit> Glyphs { get; init; } = [];

    /// <summary><see cref="Glyphs"/>, grouped into the clauses they were tokenized from - see <see cref="LineClause"/>.</summary>
    public List<LineClause> Clauses { get; }

    /// <summary>Every <see cref="BackReference"/> on this line, in text order, with what it was resolved to - see <see cref="BackReferenceResolver"/>.</summary>
    public List<BackReferenceResolution> BackReferences { get; }

    /// <summary>
    /// A list of all full spans found on this specific line.
    /// </summary>
    public List<UnmatchedTextOccurrence> UnmatchedTextOccurrences { get; init; }

    public string DataPath { get; init; }

    /// <summary>What tokenizing this line tried - what tells whether another grammar could tokenize it differently (see <see cref="TokenizationChange"/>).</summary>
    public TokenizationTrace TokenizationTrace { get; init; }

    public List<RootCaptureTrace> CaptureTraceRoots => Glyphs.Select(x => x.CaptureContext.RootCaptureTrace).ToList();
    /// <summary>
    /// Just the roots that came from actually matching a Glyph type - what a consumer displaying captures
    /// as captures wants, as opposed to <see cref="CaptureTraceRoots"/>, which includes the Tokenizer's
    /// own synthesized fillers (see <see cref="RootCaptureTrace.IsSynthesized"/>) because rendering the
    /// line's text needs every span, matched or not.
    /// </summary>
    public List<RootCaptureTrace> MatchedCaptureTraceRoots => CaptureTraceRoots.Where(x => !x.IsSynthesized).ToList();

    /// <summary>
    /// Total count of all words on this line.
    /// </summary>
    public int WordCount { get; }

    /// <summary>
    /// Count of this line's words (as <see cref="WordCount"/> counts them) that no <see cref="UnmatchedString"/> span
    /// touches - so it equals <see cref="WordCount"/> exactly when the line has no unmatched text.
    /// </summary>
    public int CapturedWordCount { get; }

    public ProcessedLine(SourceTextDTO sourceText, List<CaptureUnit> glyphs, List<UnmatchedTextOccurrence> unmatchedTextOccurrences, string dataPath)
    {
        SourceText = sourceText;
        Glyphs = glyphs;
        Clauses = LineClause.Group(glyphs);
        BackReferences = BackReferenceResolver.Collect(glyphs);
        UnmatchedTextOccurrences = unmatchedTextOccurrences;
        DataPath = dataPath;

        WordCount = CountWords(sourceText.FormattedText);
        CapturedWordCount = WordCount - CountUnmatchedWords(sourceText.FormattedText, glyphs);
    }

    static int CountWords(string text) =>
        text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;

    /// <summary>
    /// How many of <paramref name="text"/>'s words any unmatched span touches. Counted over the line's own words rather
    /// than summed per token: tokens don't split where words do ("investigate.(create" is one word but a glyph, a
    /// clause break and another glyph), so counting each token's words would credit one word several times over.
    /// </summary>
    static int CountUnmatchedWords(string text, List<CaptureUnit> units)
    {
        var unmatched = new bool[text.Length];

        foreach (var trace in units.Select(x => x.CaptureContext.RootCaptureTrace).Where(x => x.IsUnmatchedString))
            Array.Fill(unmatched, true, trace.Index, trace.Length);

        int count = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
                continue;

            bool touched = false;

            for (; i < text.Length && text[i] != ' '; i++)
                touched |= unmatched[i];

            if (touched)
                count++;
        }

        return count;
    }

    /// <param name="reuse">
    /// The line already processed for a line index and its text, where it would tokenize the same under
    /// <paramref name="grammar"/> - else null, to tokenize it. Null to tokenize every line.
    /// </param>
    public static List<ProcessedLine> GetAll(IDocument document, GlyphGrammar grammar, Func<int, string, ProcessedLine> reuse = null)
    {
        var formattedLines = document.GetFormattedLines();
        List<ProcessedLine> lines = [];

        for (int i = 0; i < formattedLines.Length; i++)
        {
            var formattedText = formattedLines[i];

            if (string.IsNullOrWhiteSpace(formattedText))
                continue;

            if (reuse?.Invoke(i, formattedText) is { } reused)
            {
                lines.Add(reused);
                continue;
            }

            SourceTextDTO sourceText = new(formattedText, document.Name, i);

            List<CaptureUnit> lineGlyphs = null;
            var trace = TokenizationTrace.Record(() => lineGlyphs = grammar.Tokenize(sourceText.FormattedText));
            var unmatchedTextOccurrences = GetUnmatchedStringOccurrences(document, lineGlyphs, i);
            var dataPath = document.Name.Replace(' ', '_') + $"-line[{i}]";
            ProcessedLine processedLine = new(sourceText, lineGlyphs, unmatchedTextOccurrences, dataPath) { TokenizationTrace = trace };
            lines.Add(processedLine);
        }

        return lines;
    }

    /// <summary>
    /// A depth-first, positionally-equidistant rainbow color per <see cref="CaptureTrace"/> on this
    /// line, skipping any node <paramref name="isCollapsed"/> reports as collapsed when handing out
    /// slots (its children are still walked and may themselves get one) — so a caller hiding
    /// collapsed nodes gets the full spread of hues spent only on what's actually shown, instead of
    /// losing slots to nodes with no border to paint. Deliberately uncached and recomputed on every
    /// call: this line's data is shared, singleton-lifetime, and "which nodes are collapsed" is a
    /// per-viewer display preference, not a fact about the document, so nothing here can be baked
    /// in once and reused across viewers/settings.
    /// </summary>
    public Dictionary<CaptureTrace, HexPalette> GetPositionalPalettes(Func<CaptureTrace, bool> isCollapsed)
    {
        var visibleOrder = new List<CaptureTrace>();

        void Walk(CaptureTrace captureTrace)
        {
            if (!isCollapsed(captureTrace))
                visibleOrder.Add(captureTrace);

            // One slot per distinct named group (Children), not per repeat occurrence - every
            // occurrence of a group nested inside a repeated ancestor is the same semantic capture
            // slot, just at a different text position, so they all share this one hue (see
            // CaptureTrace.Representative and its use in SpanView/PropertyTable's own palette lookups).
            foreach (var child in captureTrace.Children)
                Walk(child);
        }

        foreach (var root in CaptureTraceRoots)
            Walk(root);

        var paletteSet = DeterministicPalette.GetPositionalPaletteSet(visibleOrder.Count);

        return visibleOrder
            .Select((captureTrace, index) => (captureTrace, index))
            .ToDictionary(x => x.captureTrace, x => paletteSet[x.index]);
    }

    static List<UnmatchedTextOccurrence> GetUnmatchedStringOccurrences(IDocument document, List<CaptureUnit> lineGlyphs, int lineIndex)
    {
        var occurrences = new List<UnmatchedTextOccurrence>();

        for (int i = 0; i < lineGlyphs.Count; i++)
        {
            var glyph = lineGlyphs[i];

            // Check for and record unmatched tokens
            // Create a new occurrence, giving it the context of the entire line's tokens.
            if (glyph is UnmatchedString)
                occurrences.Add(new UnmatchedTextOccurrence(document.Name, lineIndex, lineGlyphs, i));
        }

        return occurrences;
    }
}