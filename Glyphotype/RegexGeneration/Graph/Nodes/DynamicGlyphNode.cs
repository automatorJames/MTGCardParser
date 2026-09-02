
namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a <see cref="DynamicGlyph"/> property: matches greedily against anything up to the next
/// sentence boundary, then re-tokenizes the matched text (optionally scoped by <see cref="TypeFilterAttribute"/>)
/// to resolve which concrete <see cref="Glyph"/> type it actually represents.
/// </summary>
public class DynamicGlyphNode : GlyphNode
{
    protected override string DefaultPattern => @"[^.]+";

    public DynamicGlyphNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        if (Navigation.Patterns != null && Navigation.Patterns.Any())
            Navigation.Patterns.ToList().ForEach(x => children.Add(new TextNode(this, x)));
        else
            children.Add(new TextNode(this, DefaultPattern));
    }

    /// <inheritdoc/>
    public override bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph)
    {
        glyph = null;
        Type filterType = Navigation.Prop?.GetCustomAttribute<TypeFilterAttribute>()?.Type ?? typeof(Glyph);
        var captureValue = captureTrace.CaptureValue;
        var resolvedTokens = GlyphTypeRegistry.ClassTokenizer.Tokenize(captureValue, scopeToType: filterType, includeDependentTypes: true);

        // Dynamic match tokens must not begin with unmatched text, and must contain at least one real match
        if (resolvedTokens.FirstOrDefault() is UnmatchedString || resolvedTokens.OfType<Glyph>().FirstOrDefault() is not Glyph dynamicMatchToken)
            return false;

        // This node's pattern is greedy (see DefaultPattern), so the text it captured routinely runs well
        // past the single Glyph the re-tokenization above actually resolved out of it. Accepting that
        // as-is is what would let the enclosing match claim - all the way back up to Tokenizer, which
        // advances by the whole matched token's length - every character through the end of this capture
        // while having really only accounted for this one resolved prefix of it.
        if (dynamicMatchToken.CaptureValue.Length < captureValue.Length)
        {
            RequestNarrowingIfTrailing(captureTrace, dynamicMatchToken.CaptureValue.Length);
            return false;
        }

        glyph = new DynamicGlyph(dynamicMatchToken)
        {
            CaptureContext = dynamicMatchToken.CaptureContext
        };

        // The re-tokenization above ran against captureValue in isolation, producing its own
        // disconnected CaptureContext/RootCaptureTrace. Without this, this node's CaptureTrace
        // would stay childless — a flat leaf with no visibility into dynamicMatchToken's own
        // resolved structure
        captureTrace.AdoptDynamicChildren(dynamicMatchToken.CaptureContext.RootCaptureTrace);

        return true;
    }

    /// <summary>
    /// Handles a resolution that stopped short of this node's own capture, by asking
    /// <see cref="RegexGraph.TryMatch(string, int, int, out Glyph)"/> to retry the whole enclosing match
    /// against a scope ending exactly where the resolution did - but only when this capture is the last
    /// thing in that match, since only then is the shortfall a trailing remainder the Tokenizer can pick
    /// up again at the next whole word. A shortfall anywhere else leaves a hole with already-matched text
    /// on both sides of it, and nothing could ever fill it: a DynamicGlyph is only ever asked to resolve
    /// one thing, so if that one thing doesn't reach the next required nib, the enclosing match simply
    /// isn't a real match. Either way this node's own hydration fails - recording the narrowing is purely
    /// what tells those two cases apart.
    /// </summary>
    void RequestNarrowingIfTrailing(CaptureTrace captureTrace, int resolvedLength)
    {
        if (captureTrace.End != captureTrace.CaptureContext.RootCaptureTrace.End)
            return;

        captureTrace.CaptureContext.RequestNarrowedScopeEnd(captureTrace.Index + resolvedLength);
    }
}