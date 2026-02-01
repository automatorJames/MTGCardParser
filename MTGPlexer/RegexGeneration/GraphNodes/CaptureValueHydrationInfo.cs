namespace MTGPlexer.RegexGeneration.GraphNodes;

public record CaptureValueHydrationInfo
{
    public CaptureNode CaptureNode { get; set; }
    public object Value { get; }
    public string FullyQualifiedName { get; }
    public string CaptureText { get; }
    public int Index { get; }
    public int Length { get; }


    public CaptureValueHydrationInfo(CaptureNode captureNode, Capture capture, object value)
    {
        CaptureNode = captureNode;
        Value = value;
        FullyQualifiedName = captureNode.FullyQualifiedName;
        CaptureText = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
    }
};

public enum CaptureValueResult
{
    FoundWithValue,
    FoundButNull,
    NameNotFound,
    Exception
}
