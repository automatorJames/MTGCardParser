
namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a <see cref="DynamicGlyph"/> property: matches greedily against anything up to the next
/// sentence boundary, then re-tokenizes the matched text (optionally scoped by <see cref="TypeFilterAttribute"/>)
/// to resolve which concrete <see cref="Glyph"/> type it actually represents.
/// </summary>
public class DynamicGlyphNode : GlyphNode
{
    protected override string DefaultPattern => @"[^.]+";
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Dynamic;

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
        var resolvedTokens = GlyphTypeRegistry.ClassTokenizer.Tokenize(captureValue, scopeToType: filterType);

        // Dynamic match tokens must not begin with unmatched text, and must contain at least one real match
        if (resolvedTokens.FirstOrDefault() is UnmatchedString || resolvedTokens.OfType<Glyph>().FirstOrDefault() is not Glyph dynamicMatchToken)
            return false;

        glyph = new DynamicGlyph(dynamicMatchToken);
        glyph.CaptureContext = dynamicMatchToken.CaptureContext;

        // The re-tokenization above ran against captureValue in isolation, producing its own
        // disconnected CaptureContext/RootCaptureTrace. Without this, this node's CaptureTrace
        // would stay childless — a flat leaf with no visibility into dynamicMatchToken's own
        // resolved structure (e.g. a DynamicGlyph resolving to ThisDealsDamage would show no
        // Quantity/Recipient nesting at all in the underline tree or corpus-wide lookups).
        captureTrace.AdoptDynamicChildren(dynamicMatchToken.CaptureContext.RootCaptureTrace);

        return true;
    }
}