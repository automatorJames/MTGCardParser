using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;

namespace Glyphotype.RegexGeneration.Graph;

[JsonObject(MemberSerialization.OptIn)]
public class CaptureTrace : IEnumerable<CaptureTrace>
{
    public CaptureContext CaptureContext { get; private set; }
    [JsonProperty] public string FullyQualifiedName { get; private set; }
    [JsonProperty] public string Name { get; }

    /// <summary>
    /// The graph node that produced this capture - unlike <see cref="FullyQualifiedName"/>, this is never
    /// touched by <see cref="Rebase"/>, so it stays a stable, reliable identity for a dynamic capture's
    /// re-tokenized, rebased descendants (see <see cref="AdoptDynamicChildren"/>) even after their display
    /// name has been rewritten to read as part of the outer graph's path. <see cref="RegexGraph.NamedGroupFlatGraph"/>
    /// only knows this node by its own (un-rebased) graph, so consumers that need to color a capture the
    /// same way its origin type's own formatted regex does (e.g. <see cref="Presentation.MatchContentRenderer"/>)
    /// should key off this instead of the display-only FullyQualifiedName.
    /// </summary>
    public NamedGroupNode SourceNode { get; }
    [JsonProperty] public string CaptureValue { get; set; }
    public string PrintValue => GetPrintValue();
    public bool Success { get; }
    [JsonProperty] public CaptureNodeKind NodeKind { get; }
    public Type ResolvedNodeType => GetResolvedNodeType();
    [JsonProperty] public string ResolvedNodeTypeName => ResolvedNodeType.Name;
    public string ParentName { get; private set; }
    [JsonProperty] public int Index { get; private set; }
    [JsonProperty] public int Length { get; }
    [JsonProperty] public int End { get; private set; }
    [JsonProperty] public int? SiblingIndex { get; }
    [JsonProperty] public int Count => (Success ? 1 : 0) + Siblings.Count;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public List<CaptureTrace> Siblings { get; } = [];
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public List<CaptureTrace> Children { get; } = [];
    public object ClrValue { get; set; }
    [JsonProperty] public bool IsTerminal { get; }

    /// <summary>
    /// Node kinds that always represent a meaningful resolution — a choice among named
    /// alternatives (<see cref="CaptureNodeKind.OneOf"/>) or a re-tokenized dynamic match
    /// (<see cref="CaptureNodeKind.Dynamic"/>) — even when what they resolve to isn't itself a
    /// scalar. These never collapse, regardless of their children.
    /// </summary>
    static readonly HashSet<CaptureNodeKind> _alwaysMeaningfulKinds = [CaptureNodeKind.OneOf, CaptureNodeKind.Dynamic];

    /// <summary>
    /// True when this node is a pure pass-through: a single named child that itself carries no
    /// direct scalar value, and this node isn't one of <see cref="_alwaysMeaningfulKinds"/>.
    /// Displaying such a node's own underline level would be visual noise — it never resolved a
    /// choice and never aggregated more than one property, it just forwards to its one child.
    /// A node with two or more children is never collapsible (it's aggregating distinct named
    /// properties, which is itself meaningful structure), and a node whose only child is
    /// terminal is never collapsible (the terminal leaf's overline needs this level's underline
    /// to pair with).
    /// </summary>
    [JsonProperty]
    public bool IsCollapsible =>
        Children.Count == 1
        && !Children[0].IsTerminal
        && !_alwaysMeaningfulKinds.Contains(NodeKind);

    /// <summary>
    /// True when this node should draw its own underline: it has children of its own to bracket, or (as
    /// a <see cref="RootCaptureTrace"/>) has no enclosing parent to draw one on its behalf. A non-root
    /// leaf paints none of its own, relying on its parent's underline to show it matched - but a leaf
    /// root has no such parent, so nothing else in the tree would ever represent it (e.g. a top-level
    /// Glyph whose only Nib is a literal string). Collapsing (see <see cref="IsCollapsible"/>) is a
    /// separate, display-preference-dependent concern layered on top by the viewer.
    /// </summary>
    public bool HasOwnBoundary =>
        Children.Count > 0 || this is RootCaptureTrace;

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
        SourceNode = namedGroupNode;

        var parentNameMatch = Regex.Match(FullyQualifiedName, @"^.+(?=_[^_]+$)");

        if (parentNameMatch.Success && !string.IsNullOrWhiteSpace(parentNameMatch.Value))
            ParentName = parentNameMatch.Value;

        CaptureContext = captureContext;
    }

    public IEnumerator<CaptureTrace> GetEnumerator()
    {
        if (Success)
            yield return this;

        foreach (var sibling in Siblings)
            yield return sibling;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The deepest count of "meaningful" nesting levels beneath (and including) this node, per
    /// <paramref name="isCollapsed"/> — a node this predicate reports as collapsed doesn't count
    /// as a level of its own (it contributes no visible underline/border of its own to stack),
    /// but its children are still walked and may count. Passing a predicate that always returns
    /// false recovers plain structural depth.
    /// </summary>
    public int GetEffectiveDepth(Func<CaptureTrace, bool> isCollapsed)
    {
        if (Children.Count == 0)
            return 0;

        var deepestChild = Children.Max(child => child.GetEffectiveDepth(isCollapsed));
        return isCollapsed(this) ? deepestChild : 1 + deepestChild;
    }

    /// <summary>
    /// Adopts <paramref name="innerRoot"/>'s children as this node's own children, in place of the
    /// flat, structure-less capture a <see cref="Nodes.DynamicGlyphNode"/> otherwise produces.
    /// <paramref name="innerRoot"/> is the <see cref="RootCaptureTrace"/> produced by re-tokenizing
    /// this node's matched text in isolation (its own <see cref="CaptureContext"/>, with indexes
    /// relative to that substring rather than to the original line) — so every adopted descendant
    /// is rebased onto this node's own <see cref="CaptureContext"/> and index space, and re-parented
    /// under this node's <see cref="FullyQualifiedName"/> plus the resolved type's own root name
    /// (replacing <paramref name="innerRoot"/>'s own name as the path prefix, but keeping it as a
    /// trailing segment rather than dropping it) so its data-path stays fully qualified from the line
    /// root, its captures stay registered for corpus-wide lookups (<see cref="RootCaptureTrace.this[string]"/>),
    /// and - the reason the resolved type's name is kept rather than dropped - it lines up exactly
    /// with the container path <see cref="Presentation.DynamicSectionBuilder"/> independently builds
    /// for this same resolved type's embedded section in the formatted regex output, so a
    /// <see cref="Presentation.MatchContentRenderer"/> span's data-path matches the pre's for the same
    /// capture instead of the two diverging.
    /// </summary>
    public void AdoptDynamicChildren(RootCaptureTrace innerRoot)
    {
        var outerRoot = CaptureContext.RootCaptureTrace;
        var oldPrefix = innerRoot.FullyQualifiedName;
        var newPrefix = $"{FullyQualifiedName}_{oldPrefix}";

        foreach (var child in innerRoot.Children)
        {
            child.Rebase(oldPrefix, newPrefix, Index, CaptureContext);
            Children.Add(child);
            outerRoot.RegisterSubtree(child);
        }
    }

    void Rebase(string oldFullyQualifiedNamePrefix, string newFullyQualifiedNamePrefix, int indexOffset, CaptureContext newContext)
    {
        FullyQualifiedName = newFullyQualifiedNamePrefix + FullyQualifiedName[oldFullyQualifiedNamePrefix.Length..];

        var parentNameMatch = Regex.Match(FullyQualifiedName, @"^.+(?=_[^_]+$)");
        ParentName = parentNameMatch.Success ? parentNameMatch.Value : null;

        Index += indexOffset;
        End += indexOffset;
        CaptureContext = newContext;

        foreach (var child in Children)
            child.Rebase(oldFullyQualifiedNamePrefix, newFullyQualifiedNamePrefix, indexOffset, newContext);

        foreach (var sibling in Siblings)
            sibling.Rebase(oldFullyQualifiedNamePrefix, newFullyQualifiedNamePrefix, indexOffset, newContext);
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

        if (ClrValue is DynamicGlyph dynamicGlyph)
            return dynamicGlyph.ResolvedType;

        if (ClrValue is OneOfBase oneOf)
            return oneOf.GetResolvedType();

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
    CompoundOf,
    Optional,
    Dynamic,
    Enum,
    Int,
    Bool,
    Internals
}