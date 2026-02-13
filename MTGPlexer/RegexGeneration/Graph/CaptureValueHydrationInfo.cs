namespace MTGPlexer.RegexGeneration.Graph;

public record CaptureValueHydrationInfo
{
    public NamedGroupNode NamedGroupNode { get; set; }
    public string FullyQualifiedName { get; }
    public string CaptureText { get; }
    public int Index { get; }
    public int Length { get; }
    public object Value { get; }

    public CaptureValueHydrationInfo(NamedGroupNode namedGroupNode, Capture capture, object value)
    {
        if (capture is null)
            throw new ArgumentNullException(nameof(capture));

        NamedGroupNode = namedGroupNode;
        FullyQualifiedName = namedGroupNode.FullyQualifiedName;
        CaptureText = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
        Value = value;
    }
};