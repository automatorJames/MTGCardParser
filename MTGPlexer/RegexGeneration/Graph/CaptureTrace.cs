using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;

namespace MTGPlexer.RegexGeneration.Graph;

[JsonObject(MemberSerialization.OptIn)]
public class CaptureTrace : IEnumerable<CaptureTrace>
{
    public CaptureContext CaptureContext { get; }
    [JsonProperty] public string FullyQualifiedName { get; }
    [JsonProperty] public string Name { get; }
    [JsonProperty] public string CaptureValue { get; set; }
    public string PrintValue => GetPrintValue();
    public bool Success { get; }
    [JsonProperty] public CaptureNodeKind NodeKind { get; }
    public Type ResolvedNodeType => GetResolvedNodeType();
    [JsonProperty] public string ResolvedNodeTypeName => ResolvedNodeType.Name;
    public string ParentName { get; }
    [JsonProperty] public int Index { get; }
    [JsonProperty] public int Length { get; }
    [JsonProperty] public int End { get; }
    [JsonProperty] public int? SiblingIndex { get; }
    [JsonProperty] public int Count => (Success ? 1 : 0) + Siblings.Count;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public List<CaptureTrace> Siblings { get; } = [];
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public List<CaptureTrace> Children { get; } = [];
    public object ClrValue { get; set; }
    [JsonProperty] public bool IsTerminal { get; }
    [JsonProperty] public bool IsCollapsible => Children.Count > 0 && !Children.Any(x => x.IsTerminal);

    public string JsonDebug => JsonConvert.SerializeObject(
        this,
        Formatting.Indented,
        new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            [
                new StringEnumConverter()
            ]
        });

    public bool ShouldSerializeSiblings => Siblings.Count > 0;
    public bool ShouldSerializeChildren => Children.Count > 0;
    public bool ShouldSerializeCount => Count > 1;

    public CaptureTrace(CaptureContext captureContext, NamedGroupNode namedGroupNode, Capture capture, int? siblingIndex = null)
    : this(captureContext, namedGroupNode)
    {
        if (capture is null)
            throw new ArgumentNullException(nameof(capture));

        Success = true;
        CaptureValue = capture.Value;
        Index = capture.Index;
        Length = capture.Length;
        End = Index + Length;
        SiblingIndex = siblingIndex;
    }

    public CaptureTrace(CaptureContext captureContext, NamedGroupNode namedGroupNode)
    {
        NodeKind = namedGroupNode.NodeKind;
        IsTerminal = CheckNodeTypeIsTerminal(NodeKind);
        FullyQualifiedName = namedGroupNode.FullyQualifiedName;
        Name = namedGroupNode.Name;

        var parentNameMatch = Regex.Match(FullyQualifiedName, @"^.+(?=_[^_]+$)");

        if (parentNameMatch.Success && !string.IsNullOrWhiteSpace(parentNameMatch.Value))
            ParentName = parentNameMatch.Value;

        CaptureContext = captureContext;
    }

    public CaptureTrace this[int captureIndex]
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

    public IEnumerator<CaptureTrace> GetEnumerator()
    {
        if (Success)
            yield return this;

        foreach (var sibling in Siblings)
            yield return sibling;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int GetRecursiveDepth()
    {
        if (Children.Count == 0)
            return 0;

        return 1 + Children.Max(child => child.GetRecursiveDepth());
    }

    string GetPrintValue()
    {
        if (ClrValue == null)
            return null;

        return NodeKind.ToString() + ": " + ClrValue.ToString();
    }

    Type GetResolvedNodeType()
    {
        if (ClrValue == null)
            return null;

        if (ClrValue is DynamicToken dynamicToken)
            return dynamicToken.ResolvedType;

        return ClrValue.GetType();
    }

    static bool CheckNodeTypeIsTerminal(CaptureNodeKind nodeKind) =>
        nodeKind switch
        {
            CaptureNodeKind.Enum => true,
            CaptureNodeKind.Int => true,
            CaptureNodeKind.Bool => true,
            _ => false
        };

    public override string ToString() => CaptureValue;
}

public enum CaptureNodeKind
{
    Token,
    OneOf,
    ManyOf,
    Compound,
    Optional,
    Dynamic,
    Enum,
    Int,
    Bool,
}