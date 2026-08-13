
namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a <see cref="DynamicToken"/> property: matches greedily against anything up to the next
/// sentence boundary, then re-tokenizes the matched text (optionally scoped by <see cref="TypeFilterAttribute"/>)
/// to resolve which concrete <see cref="TokenUnit"/> type it actually represents.
/// </summary>
public class DynamicTokenNode : TokenUnitNode
{
    protected override string DefaultPattern => @"[^.]+";
    protected override Joiner Joiner => Joiner.Pipe;

    public override CaptureNodeType NodeType => CaptureNodeType.Dynamic;

    public DynamicTokenNode(RegexNode parentNode, Navigation navigation)
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
    public override bool TryHydrate(CaptureTrace captureInfo, out CaptureUnit tokenUnit)
    {
        tokenUnit = null;
        Type filterType = Navigation.Prop?.GetCustomAttribute<TypeFilterAttribute>()?.Type ?? typeof(TokenUnit);
        var captureValue = captureInfo.CaptureValue;
        var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(captureValue, scopeToType: filterType);

        // Dynamic match tokens must not begin with unmatched text, and must contain at least one real match
        if (resolvedTokens.FirstOrDefault() is UnmatchedString || resolvedTokens.OfType<TokenUnit>().FirstOrDefault() is not TokenUnit dynamicMatchToken)
            return false;

        tokenUnit = new DynamicToken(dynamicMatchToken);
        tokenUnit.CaptureContext = dynamicMatchToken.CaptureContext;

        return true;
    }
}