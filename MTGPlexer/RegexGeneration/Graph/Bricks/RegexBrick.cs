namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>
/// One atomic piece of a compiled regex, tied back to the <see cref="RegexNode"/> that produced it.
/// <see cref="Regex"/> is the immutable text actually used for pattern matching (see <c>BuiltRegex.MinifiedRegex</c>);
/// it is never touched after construction. <see cref="RegexFormatted"/> and <see cref="CommentFormatted"/> are
/// display-only and are populated entirely by the Formatting layer (e.g. <c>RegexBrickFormattingPipeline</c>,
/// <c>SmartLineRenderer</c>) — nothing in the Graph layer assigns them, so there is exactly one place responsible
/// for deciding what a brick's rendered comment says.
/// </summary>
public class RegexBrick
{
    /// <summary>Adjustment applied to <see cref="NestedDepth"/> by subclasses (e.g. group bookends sit one level shallower than their contents).</summary>
    protected virtual int NestedDepthModifer => 0;

    /// <summary>The immutable regex text this brick contributes to the compiled matching pattern.</summary>
    public string Regex { get; }

    readonly string _baseFullyQualifiedName;
    string _fullyQualifiedNameOverride;

    /// <summary>The fully qualified name of the <see cref="RegexNode"/> that produced this brick, or an override set by <see cref="OverrideFullyQualifiedName"/>.</summary>
    public string FullyQualifiedName => _fullyQualifiedNameOverride ?? _baseFullyQualifiedName;

    int _depthOffset;

    /// <summary>How many enclosing named groups (excluding transparent roots) this brick is nested within.</summary>
    public int NestedDepth => _baseNestedDepth + _depthOffset;

    readonly int _baseNestedDepth;

    /// <summary>The full ancestor chain (root-to-self) of the <see cref="RegexNode"/> that produced this brick.</summary>
    public RegexNode[] NodeLineage { get; }

    /// <summary>The <see cref="RegexNode"/> that directly produced this brick.</summary>
    public RegexNode Parent => NodeLineage.LastOrDefault();

    /// <summary>The subset of <see cref="NodeLineage"/> that are named groups, root-to-self.</summary>
    public NamedGroupNode[] NamedGroupNodeLineage => NodeLineage.OfType<NamedGroupNode>().ToArray();

    NamedGroupNode _namedGroupParentOverride;

    /// <summary>The nearest enclosing named group, or an override set by <see cref="OverrideNamedGroupParent"/>.</summary>
    public NamedGroupNode NamedGroupParent => _namedGroupParentOverride ?? NamedGroupNodeLineage.LastOrDefault();

    /// <summary>Fully qualified names of every named group enclosing this brick, root-to-self.</summary>
    public string[] NamedGroupLineageNames => NamedGroupNodeLineage.Select(x => x.FullyQualifiedName).ToArray();

    /// <summary>Enclosing named groups, self-to-root, omitting transparent root groups.</summary>
    public NamedGroupNode[] GroupLineage { get; }

    /// <summary>Short names (not fully qualified) of <see cref="GroupLineage"/>, self-to-root.</summary>
    public string[] GroupLineageNames { get; }

    string _regexFormatted;

    /// <summary>Display-only regex text, defaulting to <see cref="Regex"/> until the Formatting layer overrides it (e.g. simplified group names, ranked enum members).</summary>
    public string RegexFormatted
    {
        get => _regexFormatted ?? Regex ?? "";
        set => _regexFormatted = value;
    }

    /// <summary>Display-only comment text for this brick, assigned exclusively by the Formatting layer. Empty until then.</summary>
    public string CommentFormatted { get; set; } = "";

    public RegexBrick(RegexNode parentNode, string regex)
    {
        Regex = regex;
        NodeLineage = parentNode.Lineage;
        _baseFullyQualifiedName = parentNode.FullyQualifiedName;
        GroupLineage = parentNode.Lineage.OfType<NamedGroupNode>().Where(x => !x.IsTransparentRoot).Reverse().ToArray();
        GroupLineageNames = GroupLineage.Select(x => x.Name).ToArray();
        _baseNestedDepth = CalculateNestedDepth();
    }

    /// <summary>Computes the base <see cref="NestedDepth"/> from <see cref="GroupLineage"/>, adjusted by <see cref="NestedDepthModifer"/>.</summary>
    protected virtual int CalculateNestedDepth() =>
        GroupLineage.Length + NestedDepthModifer;

    /// <summary>
    /// Sets (does not accumulate) an offset applied on top of this brick's own graph-computed depth. A
    /// brick's depth is normally fixed by its own graph's ancestry, but a brick spliced in from an
    /// independently-compiled graph — e.g. a dynamic capture's resolved type, embedded by
    /// <see cref="DynamicSectionBuilder"/> — needs its depth shifted to match where it's actually being
    /// rendered. Bricks are shared/cached per type and re-formatted on every render, so this must overwrite
    /// rather than add — an additive offset would compound further on every subsequent render.
    /// </summary>
    public void OffsetNestedDepth(int levels) => _depthOffset = levels;

    /// <summary>
    /// Overrides <see cref="FullyQualifiedName"/>, e.g. rebasing a dynamic capture's resolved type's own
    /// "DrawOrDiscardCards_CardVerb" onto where it's actually nested — "TargetPlayerAction_Action_DrawOrDiscardCards_CardVerb"
    /// — so it reads as a real (if runtime-resolved) part of the outer graph's data-path instead of a
    /// disconnected one.
    /// </summary>
    public void OverrideFullyQualifiedName(string fullyQualifiedName) => _fullyQualifiedNameOverride = fullyQualifiedName;

    /// <summary>
    /// Overrides <see cref="NamedGroupParent"/> — the identity coloring and box-character lookups key off
    /// of — so an embedded resolved type's own container bookends get their own distinct rainbow color
    /// (its <see cref="RegexNode.Lineage"/> root) instead of inheriting the enclosing dynamic group's.
    /// </summary>
    public void OverrideNamedGroupParent(NamedGroupNode namedGroupParent) => _namedGroupParentOverride = namedGroupParent;

    /// <summary>
    /// Clears every embedding override back to this brick's own graph-computed identity/depth. Bricks are
    /// shared/cached per type and re-formatted on every render, so a plain (non-embedded) formatting pass
    /// must call this for every brick it touches — otherwise a type rendered on its own page could inherit
    /// stale overrides from the last time it was embedded inside some other type's dynamic capture.
    /// </summary>
    public void ResetEmbeddingOverrides()
    {
        _depthOffset = 0;
        _fullyQualifiedNameOverride = null;
        _namedGroupParentOverride = null;
    }

    public override string ToString() => RegexFormatted;
}
