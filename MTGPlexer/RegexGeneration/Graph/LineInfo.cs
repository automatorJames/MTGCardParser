namespace MTGPlexer.RegexGeneration.Graph;

public record LineInfo
{
    public string SourceText { get; set; }
    public int LineNumber { get; set; }
    public List<CaptureInfo> Captures { get; set; } = [];

    public LineInfo(string sourceText, int lineNumber, List<CaptureInfo> captures)
    {
        SourceText = sourceText;
        LineNumber = lineNumber;
        Captures = captures;
    }
}
