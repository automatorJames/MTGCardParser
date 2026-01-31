namespace MTGPlexer.RegexGeneration.GraphNodes;

public record CaptureValueInfo
{
    public CaptureValueResult Result { get; }
    public object Value { get; }
    public string FullyQualifiedCaptureName { get; }
    public string CaptureText { get; }
    public int Index { get; }
    public int Length { get; }
    public int Ordinal { get; }

    public CaptureValueInfo(CaptureValueResult result)
    {
        if (result == CaptureValueResult.FoundWithValue)
            throw new Exception($"This constuctor may not be used when a capture is found");

        Index = -1;
        Length = -1;
    }

    public CaptureValueInfo(object value, string fullyQualifiedCaptureName, Capture capture, int ordinal = 0)
    {
        Result = CaptureValueResult.FoundWithValue;
        Value = value;
        FullyQualifiedCaptureName = fullyQualifiedCaptureName;
        CaptureText = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
        Ordinal = ordinal;
    }
};

public enum CaptureValueResult
{
    FoundWithValue,
    FoundButNull,
    NameNotFound,
    Exception
}
