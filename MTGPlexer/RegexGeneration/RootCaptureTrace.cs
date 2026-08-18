using Newtonsoft.Json;

namespace MTGPlexer.RegexGeneration.Graph;

[JsonObject(MemberSerialization.OptIn)]
public class RootCaptureTrace : CaptureTrace
{
    Dictionary<string, CaptureTrace> _flatCaptureTree { get; } = [];

    public TokenUnitNode RootNode { get; }
    [JsonProperty] public bool IsUnmatchedString { get; }

    public RootCaptureTrace(CaptureContext captureContext, TokenUnitNode rootNode, Capture capture)
        : base(captureContext, rootNode, capture)
    {
        RootNode = rootNode;
        _flatCaptureTree[rootNode.FullyQualifiedName] = this;
        IsUnmatchedString = rootNode is UnmatchedTokenUnitNode;
    }

    public CaptureTrace this[string fullyQualifiedName]
    {
        get
        {
            if (_flatCaptureTree.TryGetValue(fullyQualifiedName, out var captureTrace))
                return captureTrace;

            return null;
        }
    }

    public void AddCaptureTrace(CaptureTrace captureTrace)
    {
        _flatCaptureTree[captureTrace.FullyQualifiedName] = captureTrace;

        if (!_flatCaptureTree.TryGetValue(captureTrace.ParentName, out var parentCaptureTrace))
            throw new Exception($"Found no {nameof(CaptureTrace)} parent named \"{captureTrace.ParentName}\" for child \"{captureTrace.FullyQualifiedName}\"");

        parentCaptureTrace.Children.Add(captureTrace);
    }

    public Dictionary<string, CaptureTrace> GetFlatCaptureTree() => _flatCaptureTree;
}