namespace MTGPlexer.RegexGeneration.GraphNodes;

public record CaptureValueInfo
{
    public ValueNode Node { get; set; }
    public CaptureValueResult Result { get; }
    public object Value { get; }
    public string FullyQualifiedCaptureName { get; }
    public string CaptureText { get; }
    public int Index { get; }
    public int Length { get; }
    public int Ordinal { get; init; }
    public int SiblingCaptureCount { get; init; }
    public List<CaptureValueInfo> ChildCaptureValueInfos { get; } = [];

    public CaptureValueInfo(ValueNode node, CaptureValueResult result)
    {
        if (result == CaptureValueResult.FoundWithValue)
            throw new Exception($"This constuctor may not be used when a capture is found");

        Node = node;
        Index = -1;
        Length = -1;
        Ordinal = -1;
        SiblingCaptureCount = -1;
    }

    public CaptureValueInfo(ValueNode node, object value, string fullyQualifiedCaptureName, Capture capture)
    {
        Node = node;
        Result = CaptureValueResult.FoundWithValue;
        Value = value;
        FullyQualifiedCaptureName = fullyQualifiedCaptureName;
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
