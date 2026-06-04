using System.Collections;

namespace MTGPlexer.RegexGeneration.Graph;

public class CaptureInfo : IEnumerable<CaptureInfo>
{
    public CaptureContext CaptureContext { get; }
    public bool Success { get; }
    public string NodeTypeName { get; }
    public string FullyQualifiedName { get; }
    public string ParentName { get; }
    public int Index { get; }
    public int Length { get; }
    public int? SiblingIndex { get; }
    public List<CaptureInfo> Siblings { get; } = [];
    public List<CaptureInfo> Children { get; } = [];
    public string CaptureValue { get; set; }
    public object ClrValue { get; set; }
    public int Count => (Success ? 1 : 0) + Siblings.Count;

    public CaptureInfo(CaptureContext captureContext, NamedGroupNode namedGroupNode)
    {
        Success = false;
        NodeTypeName = namedGroupNode.GetType().Name;
        FullyQualifiedName = namedGroupNode.FullyQualifiedName;

        var parentNameMatch = Regex.Match(FullyQualifiedName, @"^.+(?=_[^_]+$)");

        if (parentNameMatch.Success && !string.IsNullOrWhiteSpace(parentNameMatch.Value))
            ParentName = parentNameMatch.Value;

        CaptureContext = captureContext;
    }

    public CaptureInfo(CaptureContext captureContext, NamedGroupNode namedGroupNode, Capture capture, int? siblingIndex = null)
        : this(captureContext, namedGroupNode)
    {
        if (capture is null)
            throw new ArgumentNullException(nameof(capture));

        Success = true;
        CaptureValue = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
        SiblingIndex = siblingIndex;
    }

    public CaptureInfo this[int captureIndex]
    {
        get
        {
            if (captureIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(captureIndex));

            if (captureIndex == 0)
                return this;

            return Siblings[captureIndex - 1];
        }
    }

    public IEnumerator<CaptureInfo> GetEnumerator()
    {
        if (Success)
            yield return this;

        foreach (var sibling in Siblings)
            yield return sibling;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => CaptureValue;
};