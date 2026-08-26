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

    /// <summary>
    /// A list of all full spans found on this specific line.
    /// </summary>
    public List<UnmatchedTextOccurrence> UnmatchedTextOccurrences { get; init; }

    public string DataPath { get; init; }

    public List<RootCaptureTrace> CaptureTraceRoots => Glyphs.Select(x => x.CaptureContext.RootCaptureTrace).ToList();
    public List<RootCaptureTrace> CaptureTraceRootsExceptUnmatchedStrings => CaptureTraceRoots.Where(x => !x.IsUnmatchedString).ToList();

    /// <summary>
    /// Total count of all words on this line.
    /// </summary>
    public int WordCount { get; }

    /// <summary>
    /// Count of all words captured by a matched <see cref="RootCaptureTrace"/> on this line
    /// (i.e. excluding words belonging to <see cref="UnmatchedString"/> spans).
    /// </summary>
    public int CapturedWordCount { get; }

    public ProcessedLine(SourceTextDTO sourceText, List<CaptureUnit> glyphs, List<UnmatchedTextOccurrence> unmatchedTextOccurrences, string dataPath)
    {
        SourceText = sourceText;
        Glyphs = glyphs;
        UnmatchedTextOccurrences = unmatchedTextOccurrences;
        DataPath = dataPath;

        WordCount = CountWords(sourceText.FormattedText);
        CapturedWordCount = glyphs
            .Where(x => !x.CaptureContext.RootCaptureTrace.IsUnmatchedString)
            .Sum(x => CountWords(x.CaptureValue));
    }

    static int CountWords(string text) =>
        text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;

    public static List<ProcessedLine> GetAll(IDocument document)
    {
        var formattedLines = document.GetFormattedLines();
        List<ProcessedLine> lines = [];
        
        for (int i = 0; i < formattedLines.Length; i++)
        {
            var formattedText = formattedLines[i];

            if (string.IsNullOrWhiteSpace(formattedText))
                continue;

            SourceTextDTO sourceText = new(formattedText, document.Name, i);

            var lineGlyphs = GlyphTypeRegistry.Tokenize(sourceText.FormattedText);
            var unmatchedTextOccurrences = GetUnmatchedStringOccurrences(document, lineGlyphs, i);
            var dataPath = document.Name.Replace(' ', '_') + $"-line[{i}]";
            ProcessedLine processedLine = new(sourceText, lineGlyphs, unmatchedTextOccurrences, dataPath);
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