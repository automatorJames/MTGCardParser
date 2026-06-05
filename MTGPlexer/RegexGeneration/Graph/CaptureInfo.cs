using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;

namespace MTGPlexer.RegexGeneration.Graph;

[JsonObject(MemberSerialization.OptIn)]
public class CaptureInfo : IEnumerable<CaptureInfo>
{
    public CaptureContext CaptureContext { get; }
    [JsonProperty] public string FullyQualifiedName { get; }
    [JsonProperty] public string Name { get; }
    [JsonProperty] public string CaptureValue { get; set; }
    public string PrintValue => GetPrintValue();
    public bool Success { get; }
    [JsonProperty] public CaptureNodeType NodeType { get; }
    public string ParentName { get; }
    [JsonProperty] public int Index { get; }
    [JsonProperty] public int Length { get; }
    [JsonProperty] public int End { get; }
    [JsonProperty] public int? SiblingIndex { get; }
    [JsonProperty] public int Count => (Success ? 1 : 0) + Siblings.Count;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public List<CaptureInfo> Siblings { get; } = [];
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public List<CaptureInfo> Children { get; } = [];
    public object ClrValue { get; set; }

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

    public bool ShouldSerializeSiblings() => Siblings.Count > 0;
    public bool ShouldSerializeChildren() => Children.Count > 0;
    public bool ShouldSerializeCount() => Count > 1;

    public CaptureInfo(CaptureContext captureContext, NamedGroupNode namedGroupNode)
    {
        Success = false;
        NodeType = namedGroupNode.NodeType;
        FullyQualifiedName = namedGroupNode.FullyQualifiedName;
        Name = namedGroupNode.Name;

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
        End = Index + Length;
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

    string GetPrintValue()
    {
        if (ClrValue == null)
            return null;

        return NodeType.ToString() + ": " + ClrValue.ToString();
    }

    public override string ToString() => CaptureValue;
};

public enum CaptureNodeType
{
    TokenUnit,
    OneOf,
    ManyOf,
    CompoundOf,
    OptionalOf,
    DynamicOf,
    Enum,
    Int,
    Bool,

}