namespace Glyphotype.RegexGeneration.Graph;

public class LineInfo
{
    public string SourceText { get; set; }
    public int LineNumber { get; set; }
    public List<CaptureTrace> CaptureTrees { get; set; } = [];

    public LineInfo(string sourceText, int lineNumber, List<CaptureTrace> captures)
    {
        SourceText = sourceText;
        LineNumber = lineNumber;
        CaptureTrees = captures;
    }
}
