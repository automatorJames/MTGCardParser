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
            if (!_flatCaptureTree.TryGetValue(fullyQualifiedName, out var captureTrace))
                throw new Exception($"{this.CaptureValue} doesn't contain the name \"{fullyQualifiedName}\"");

            return captureTrace;
        }
    }

    public void AddCaptureTrace(CaptureTrace captureTrace)
    {
        _flatCaptureTree[captureTrace.FullyQualifiedName] = captureTrace;

        if (!_flatCaptureTree.TryGetValue(captureTrace.ParentName, out var parentCaptureInfo))
            throw new Exception($"Found no {nameof(CaptureTrace)} parent named \"{captureTrace.ParentName}\" for child \"{captureTrace.FullyQualifiedName}\"");

        parentCaptureInfo.Children.Add(captureTrace);
    }

    public Dictionary<string, CaptureTrace> GetFlatCaptureTree() => _flatCaptureTree;
}