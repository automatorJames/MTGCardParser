namespace MTGPlexer.RegexGeneration.Graph;

public class LineInfo
{
    public string SourceText { get; set; }
    public int LineNumber { get; set; }
    public List<CaptureInfo> CaptureTrees { get; set; } = [];

    public LineInfo(string sourceText, int lineNumber, List<CaptureInfo> captures)
    {
        SourceText = sourceText;
        LineNumber = lineNumber;
        CaptureTrees = captures;
    }
}
