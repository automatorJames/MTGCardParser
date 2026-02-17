namespace MTGPlexer.RegexGeneration.Graph;

public record CaptureInfo
{
    public NamedGroupNode NamedGroupNode { get; }
    public string FullyQualifiedName { get; }
    public string CaptureText { get; }
    public int Index { get; }
    public int Length { get; }
    public int? SiblingIndex { get; }

    public CaptureInfo(NamedGroupNode namedGroupNode, Capture capture, int? siblingIndex = null)
    {
        if (capture is null)
            throw new ArgumentNullException(nameof(capture));

        NamedGroupNode = namedGroupNode;
        FullyQualifiedName = namedGroupNode.FullyQualifiedName;
        CaptureText = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
        SiblingIndex = siblingIndex;
    }
};