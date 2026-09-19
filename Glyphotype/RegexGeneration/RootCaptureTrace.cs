using Newtonsoft.Json;

namespace Glyphotype.RegexGeneration.Graph;

[JsonObject(MemberSerialization.OptIn)]
public class RootCaptureTrace : CaptureTrace
{
    Dictionary<string, CaptureTrace> _flatCaptureTree { get; } = [];

    public GlyphNode RootNode { get; }
    [JsonProperty] public bool IsUnmatchedString { get; }

    /// <summary>Whether this root is a <see cref="ClauseBreak"/> - a synthesized clause-separating period rather than a matched Glyph. Distinct from <see cref="IsUnmatchedString"/>: a clause break is modeled punctuation, not text still awaiting a Glyph.</summary>
    [JsonProperty] public bool IsClauseBreak { get; }

    /// <summary>
    /// Whether this root was manufactured by the Tokenizer to account for a span of source text, rather
    /// than produced by matching a Glyph type - <see cref="IsUnmatchedString"/> or
    /// <see cref="IsClauseBreak"/>. Neither has nibs, a property graph, or a registered type, so anything
    /// presenting captures *as* captures (the property tables, per-type corpus analysis) should skip them;
    /// they still carry a real span, so anything rendering the line's text still walks them.
    /// </summary>
    public bool IsSynthesized => IsUnmatchedString || IsClauseBreak;

    public RootCaptureTrace(CaptureContext captureContext, GlyphNode rootNode, Capture capture)
        : base(captureContext, rootNode, capture)
    {
        RootNode = rootNode;
        _flatCaptureTree[rootNode.FullyQualifiedName] = this;
        IsUnmatchedString = rootNode is UnmatchedGlyphNode;
        IsClauseBreak = rootNode is ClauseBreakNode;
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

    /// <summary>
    /// Registers <paramref name="node"/> and its descendants (but not its siblings — repeated
    /// captures of the same named group all share one FQN, so only the representative trace is
    /// registered) into the flat lookup table. Used after <see cref="CaptureTrace.AdoptDynamicChildren"/>
    /// re-parents a re-tokenized subtree, since that subtree bypasses <see cref="AddCaptureTrace"/>
    /// (its parent linkage is already known — it doesn't need to be resolved by name).
    /// </summary>
    public void RegisterSubtree(CaptureTrace node)
    {
        _flatCaptureTree[node.FullyQualifiedName] = node;

        foreach (var child in node.Children)
            RegisterSubtree(child);
    }
}