namespace MTGPlexer.RegexGeneration.Graph;

public class CaptureInfo
{
    public string NodeTypeName { get; }
    public string FullyQualifiedName { get; }
    public string ParentName { get; }
    public string CaptureText { get; }
    public int Index { get; }
    public int Length { get; }
    public int? SiblingIndex { get; }
    public List<CaptureInfo> Siblings { get; } = [];
    public List<CaptureInfo> Children { get; } = [];

    public CaptureInfo(NamedGroupNode namedGroupNode, Capture capture, int? siblingIndex = null)
    {
        if (capture is null)
            throw new ArgumentNullException(nameof(capture));

        NodeTypeName = namedGroupNode.GetType().Name;
        CaptureText = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
        SiblingIndex = siblingIndex;
        FullyQualifiedName = namedGroupNode.FullyQualifiedName;

        var parentNameMatch = Regex.Match(FullyQualifiedName, @"^.+(?=_[^_]+$)");

        if (parentNameMatch.Success && !string.IsNullOrWhiteSpace(parentNameMatch.Value))
            ParentName = parentNameMatch.Value;
    }
};