
namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a <see cref="DynamicToken"/> property: matches greedily against anything up to the next
/// sentence boundary, then re-tokenizes the matched text (optionally scoped by <see cref="TypeFilterAttribute"/>)
/// to resolve which concrete <see cref="TokenUnit"/> type it actually represents.
/// </summary>
public class DynamicTokenNode : TokenUnitNode
{
    protected override string DefaultPattern => @"[^.]+";

    public override CaptureNodeType NodeType => CaptureNodeType.DynamicOf;

    public DynamicTokenNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    /// <inheritdoc/>
    public override bool TryHydrate(CaptureContext captureContext, out CaptureUnit tokenUnit)
    {
        tokenUnit = null;
        Type filterType = Navigation.Prop?.GetCustomAttribute<TypeFilterAttribute>()?.Type ?? typeof(TokenUnit);
        var captureValue = captureContext[this].CaptureValue;
        var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(captureValue, scopeToType: filterType);

        // Dynamic match tokens must not begin with unmatched text, and must contain at least one real match
        if (resolvedTokens.FirstOrDefault() is UnmatchedString || resolvedTokens.OfType<TokenUnit>().FirstOrDefault() is not TokenUnit dynamicMatchToken)
            return false;

        tokenUnit = new DynamicToken(dynamicMatchToken);

        return true;
    }
}