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

    /// <summary>
    /// The one trace among this FQN's occurrences that hydration actually recursed into and gave real,
    /// registered <see cref="Children"/> - null when this trace IS that one. Hydration only ever resolves
    /// and recurses into a named group's FIRST-seen occurrence for a given FQN (see
    /// <see cref="CaptureContext.this[NamedGroupNode]"/>); every later occurrence - a true
    /// <see cref="Siblings"/> entry, or a <see cref="WithinScope"/> view narrowed from one - shares that
    /// exact same nested structure, just scoped down to its own span, so <see cref="EffectiveChildren"/>
    /// reads it from here instead of finding its own (always-empty) <see cref="Children"/>.
    /// </summary>
    public CaptureTrace Representative { get; private set; }

    /// <summary>Sets <see cref="Representative"/> - called only by <see cref="CaptureContext"/> when wiring up a true <see cref="Siblings"/> entry.</summary>
    internal void SetRepresentative(CaptureTrace representative) => Representative = representative;

    /// <summary>
    /// This occurrence's own children, each narrowed (via <see cref="WithinScope"/>) down to just the
    /// portion that actually falls within this occurrence's own span - reading from
    /// <see cref="Representative"/>'s <see cref="Children"/> when this trace has none of its own, so a
    /// repeat occurrence (a <see cref="Siblings"/> entry, or a view derived from one) presents the exact
    /// same nested structure a display walk finds on the representative, just scoped to itself. The one
    /// source of children every display-time walk (<see cref="CaptureTraceWalker"/> and friends) should
    /// use instead of raw <see cref="Children"/>, so a node nested inside a repeated ancestor renders
    /// once per real repetition instead of only for the first.
    /// </summary>
    public IEnumerable<CaptureTrace> EffectiveChildren =>
        (Representative ?? this).Children
            .Select(c => c.WithinScope(this))
            .Where(c => c.Success);

    /// <summary>
    /// The un-narrowed trace this instance was copied from, when this instance is a throwaway
    /// single-repetition view produced by <see cref="WithinScope"/> - null for every other trace,
    /// including <see cref="Siblings"/> entries (themselves original, un-copied captures). Lets
    /// <see cref="ClrValue"/>'s setter also reach the shared, cached trace that
    /// <see cref="CaptureContext.this[NamedGroupNode]"/> registers into <see cref="RootCaptureTrace"/> -
    /// the one corpus-wide analysis (e.g. <see cref="GlyphAnalysisDTOs.TypeExpressions.NamedGroupCaptureTraceSummary"/>)
    /// looks up directly by <see cref="FullyQualifiedName"/> - since that shared trace would otherwise
    /// never itself get hydrated once a descendant of a repeated ancestor is always resolved via a view.
    /// </summary>
    CaptureTrace Origin { get; }

    object _clrValue;
    public object ClrValue
    {
        get => _clrValue;
        set
        {
            _clrValue = value;

            if (Origin != null)
                Origin.ClrValue = value;
        }
    }

    [JsonProperty] public bool IsTerminal { get; }

    /// <summary>
    /// True when this trace's own named group sits beneath (or is) a <c>List&lt;&gt;</c>-typed ancestor
    /// property, checked via <see cref="SourceNode"/>'s own <see cref="RegexNode.Lineage"/> in the regex
    /// graph - not the capture tree, since "listable" is a structural fact about the property's declared
    /// shape (it could always have zero, one, or many occurrences), independent of how many times it
    /// actually occurred in this specific match.
    /// </summary>
    public bool IsListPosition => SourceNode.Lineage.OfType<GroupNode>().Any(n => n.Navigation.IsList);

    /// <summary>
    /// <see cref="FullyQualifiedName"/>, suffixed with something that distinguishes this specific
    /// occurrence, for every node along a repeated occurrence's own span - not just its terminal leaf -
    /// when <see cref="IsListPosition"/>: a terminal's own resolved value (e.g. distinguishing "flying"
    /// from "first strike" even though both otherwise share one FullyQualifiedName), or this occurrence's
    /// 1-based position for a non-terminal node in between (e.g. the <c>BuffMiddle</c> and <c>SecondPlus</c>
    /// wrapper spans <c>SpanView</c> renders around each occurrence), which has no scalar value of its own
    /// to read. Every node in one occurrence's own span needs disambiguating, not just its leaf, because
    /// <c>document-lines.js</c>'s hover highlighting collects every ancestor's own <c>data-path</c> up to
    /// the match boundary - an un-disambiguated wrapper shared by both occurrences would cross-highlight
    /// the other one's text even with its own leaf correctly told apart. Used purely for that kind of
    /// display-only identity; every other consumer (hydration caching, corpus-wide lookups) needs
    /// <see cref="FullyQualifiedName"/> itself to stay the one stable, shared identity for the group -
    /// including the property table's own single, combined branch header for a listable position, which
    /// deliberately keeps using the plain, shared name (only its own per-occurrence rows use this).
    /// </summary>
    public string HoverPath
    {
        get
        {
            if (!IsListPosition)
                return FullyQualifiedName;

            var suffix = IsTerminal && ClrValue != null ? ClrValue.ToString() : ((SiblingIndex ?? 0) + 1).ToString();
            return $"{FullyQualifiedName}_{suffix}";
        }
    }

    /// <summary>
    /// True when this node's own source node always represents a meaningful resolution — a choice among
    /// named alternatives (<see cref="GlyphOneOfNode"/>) or a re-tokenized dynamic match
    /// (<see cref="DynamicGlyphNode"/>) — even when what they resolve to isn't itself a scalar. These never
    /// collapse, regardless of their children.
    /// </summary>
    bool IsAlwaysMeaningful => SourceNode is GlyphOneOfNode or DynamicGlyphNode;

    /// <summary>
    /// True when this node is a pure pass-through: a single named child that itself carries no
    /// direct scalar value, and this node isn't <see cref="IsAlwaysMeaningful"/>.
    /// Displaying such a node's own underline level would be visual noise — it never resolved a
    /// choice and never aggregated more than one property, it just forwards to its one child.
    /// A node with two or more children is never collapsible (it's aggregating distinct named
    /// properties, which is itself meaningful structure), and a node whose only child is
    /// terminal is never collapsible (the terminal leaf's overline needs this level's underline
    /// to pair with).
    /// </summary>
    [JsonProperty]
    public bool IsCollapsible
    {
        get
        {
            var children = EffectiveChildren.ToList();
            return children.Count == 1 && !children[0].IsTerminal && !IsAlwaysMeaningful;
        }
    }

    /// <summary>
    /// True when this node should draw its own underline: it has children of its own to bracket, or (as
    /// a <see cref="RootCaptureTrace"/>) has no enclosing parent to draw one on its behalf. A non-root
    /// leaf paints none of its own, relying on its parent's underline to show it matched - but a leaf
    /// root has no such parent, so nothing else in the tree would ever represent it (e.g. a top-level
    /// Glyph whose only Nib is a literal string). Collapsing (see <see cref="IsCollapsible"/>) is a
    /// separate, display-preference-dependent concern layered on top by the viewer.
    /// </summary>
    public bool HasOwnBoundary =>
        EffectiveChildren.Any() || this is RootCaptureTrace;

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

    /// <summary>
    /// Creates a standalone view onto a single already-resolved occurrence of <paramref name="source"/>
    /// (either <paramref name="source"/> itself or one of its own <see cref="Siblings"/>) - used by
    /// <see cref="WithinScope"/> to narrow a trace merged across every repetition of an enclosing list
    /// down to just the one repetition currently being hydrated, without mutating the shared, cached
    /// trace other callers (and other repetitions) still rely on.
    /// </summary>
    CaptureTrace(CaptureTrace source)
        : this(source.CaptureContext, source.SourceNode)
    {
        Success = source.Success;
        CaptureValue = source.CaptureValue;
        Index = source.Index;
        Length = source.Length;
        End = source.End;
        SiblingIndex = source.SiblingIndex;
        Origin = source;

        // source is either the true representative (Representative == null on it, so this view must
        // point back to source itself) or another view/Sibling one hop closer to it already
        // (Representative already points at the real one) - either way this collapses to exactly one
        // hop, so EffectiveChildren never has to chase more than one Representative link.
        Representative = source.Representative ?? source;
    }

    public CaptureTrace(CaptureContext captureContext, NamedGroupNode namedGroupNode)
    {
        IsTerminal = namedGroupNode is EnumNode or IntNode or BoolNode;
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
        var children = EffectiveChildren.ToList();

        if (children.Count == 0)
            return 0;

        var deepestChild = children.Max(child => child.GetEffectiveDepth(isCollapsed));
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

    /// <summary>
    /// Narrows this trace down to just the occurrence(s) that fall within <paramref name="scope"/>'s own
    /// matched span. A group nested inside a repeated ("*"-quantified) ancestor produces one .NET capture
    /// per ancestor repetition, all sharing this node's one <see cref="FullyQualifiedName"/> - so resolving
    /// it (see <see cref="CaptureContext.this[NamedGroupNode]"/>) merges every repetition's occurrence
    /// together via <see cref="Siblings"/>. Called with the specific ancestor repetition currently being
    /// hydrated as <paramref name="scope"/>, this picks out only the occurrence(s) actually nested inside
    /// that one repetition, so a descendant of repetition N never sees repetition M's capture.
    /// </summary>
    public CaptureTrace WithinScope(CaptureTrace scope)
    {
        if (!Success)
            return this;

        var contained = this.Where(t => t.Index >= scope.Index && t.End <= scope.End).ToList();

        if (contained.Count == Count)
            return this;

        // Memoized (by value equality on child+scope, not by these particular object references) so
        // repeated narrowing of the same child to the same repetition - from hydration, then again from
        // one or more independent display-time walks - always lands on the one object that actually
        // carries whatever gets written onto it (e.g. ClrValue), instead of a fresh, disconnected copy.
        return CaptureContext.GetOrCreateScopedView(this, scope, () =>
        {
            if (contained.Count == 0)
                return new CaptureTrace(CaptureContext, SourceNode);

            var view = new CaptureTrace(contained[0]);
            view.Siblings.AddRange(contained.Skip(1));

            return view;
        });
    }

    string GetPrintValue()
    {
        if (ClrValue == null)
            return null;

        return SourceNode.GetType().Name + ": " + ClrValue.ToString();
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

    public override string ToString() => CaptureValue;

    /// <summary>
    /// Value equality by (<see cref="CaptureContext"/>, <see cref="SourceNode"/>, <see cref="Index"/>) -
    /// the same "stable, reliable identity" <see cref="SourceNode"/> already documents itself by for
    /// surviving <see cref="Rebase"/>, plus the text position that distinguishes one repetition's
    /// occurrence from another's. Needed because <see cref="WithinScope"/> (and so
    /// <see cref="EffectiveChildren"/>) freshly allocates a new view every time it's called, even for the
    /// exact same underlying occurrence - so two objects representing that one occurrence, built from
    /// two separate calls (e.g. once while computing a line's palette, again while rendering it), must
    /// still compare equal for either to be usable as a <c>Dictionary&lt;CaptureTrace, _&gt;</c> key.
    /// </summary>
    public override bool Equals(object obj) =>
        obj is CaptureTrace other
        && ReferenceEquals(CaptureContext, other.CaptureContext)
        && ReferenceEquals(SourceNode, other.SourceNode)
        && Index == other.Index;

    public override int GetHashCode() =>
        HashCode.Combine(CaptureContext, SourceNode, Index);
}