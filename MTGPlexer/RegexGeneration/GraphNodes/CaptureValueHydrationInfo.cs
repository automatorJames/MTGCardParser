namespace MTGPlexer.RegexGeneration.GraphNodes;

public record CaptureValueHydrationInfo
{
    public CaptureNode CaptureNode { get; set; }
    public string FullyQualifiedName { get; }
    public string CaptureText { get; }
    public int Index { get; }
    public int Length { get; }
    public object Value { get; }

    public CaptureValueHydrationInfo(CaptureNode captureNode, Capture capture, object value)
    {
        if (capture is null)
            throw new ArgumentNullException(nameof(capture));

        CaptureNode = captureNode;
        FullyQualifiedName = captureNode.FullyQualifiedName;
        CaptureText = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
        Value = value;
    }
};