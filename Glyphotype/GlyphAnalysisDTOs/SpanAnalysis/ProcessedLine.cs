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

    public Dictionary<CaptureTrace, int> CaptureTraceNestedDepths { get; } = [];
    public Dictionary<CaptureTrace, int> CaptureTraceDepthFirstPositions { get; } = [];
    public Dictionary<CaptureTrace, HexPalette> CaptureTracePositionalPalettes { get; } = [];
    public List<RootCaptureTrace> CaptureTraceRoots => Glyphs.Select(x => x.CaptureContext.RootCaptureTrace).ToList();
    public List<RootCaptureTrace> CaptureTraceRootsExceptUnmatchedStrings => CaptureTraceRoots.Where(x => !x.IsUnmatchedString).ToList();

    public ProcessedLine(SourceTextDTO sourceText, List<CaptureUnit> glyphs, List<UnmatchedTextOccurrence> unmatchedTextOccurrences, string dataPath)
    {
        SourceText = sourceText;
        Glyphs = glyphs;
        UnmatchedTextOccurrences = unmatchedTextOccurrences;
        DataPath = dataPath;
        glyphs.ForEach(x => RegisterPaletteAndDepthRecursive(x.CaptureContext.RootCaptureTrace));
        var paletteSet = DeterministicPalette.GetPositionalPaletteSet(CaptureTraceDepthFirstPositions.Count);
        CaptureTracePositionalPalettes = CaptureTraceDepthFirstPositions.ToDictionary(x => x.Key, x => paletteSet[x.Value]);
    }

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
            lineGlyphs.ForEach(x => processedLine.RegisterPaletteAndDepthRecursive(x.CaptureContext.RootCaptureTrace));
            lines.Add(processedLine);
        }

        return lines;
    }

    void RegisterPaletteAndDepthRecursive(CaptureTrace captureTrace, int depth = 0)
    {
        //if (captureTrace.CaptureValue.Contains("if you do, you gain 1 life")) Debugger.Break();

        //if (!captureTrace.IsCollapsible)
        //{
            CaptureTraceDepthFirstPositions[captureTrace] = CaptureTraceDepthFirstPositions.Count;
            CaptureTraceNestedDepths[captureTrace] = depth;
        //}

        foreach (var childTrace in captureTrace.Children)
            RegisterPaletteAndDepthRecursive(childTrace, depth + 1);
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

    public int GetDeepestChildDepth() => Glyphs.Max(x => x.CaptureContext.RootCaptureTrace.GetRecursiveDepth());
}