namespace MTGPlexer.RegexGeneration.GraphNodes;

public class RootNode : RegexNode
{
    static readonly char[] _boundaryChars = [' ', '.'];

    public Type RootType { get; }
    public BuiltRegex BuiltRegex { get; }
    public TokenUnitNode ContentRootNode { get;}

    public IEnumerable<CaptureNode> CaptureChildren => Children.OfType<CaptureNode>();

    public RootNode(Type type) : base(null, new TypeNavigation(type))
    {
        if (!type.IsAssignableTo(typeof(TokenUnit)))
            throw new ArgumentException($"{nameof(RootNode)} is only constructible from {nameof(TokenUnit)} types, but '{type.Name}' was passed");

        RootType = type;
        RegexBuilder regexBuilder = new(this);

        TokenUnitNode contentRoot = RootType switch
        {
            { } t when t == typeof(TokenUnitCompound) => new TokenUnitNode(this,)
        }

        // Read from cache if available, else build
        BuiltRegex = TokenTypeRegistry.RootNodes.TryGetValue(type, out var rootNode) ? rootNode.BuiltRegex : BuildRegex();
    }

    public bool TryMatch(string sourceText, out TokenUnit tokenUnit) => TryMatch(sourceText, 0, sourceText.Length, out tokenUnit);

    /// <summary>
    /// Evaluates if the source text at the current index satisfies the regex and MTG boundary rules.
    /// </summary>
    public bool TryMatch(string sourceText, int currentIndex, int endIndex, out TokenUnit tokenUnit)
    {
        tokenUnit = null;
        var match = BuiltRegex.Regex.Match(sourceText, currentIndex);

        // 1. Regex Success
        // 2. Anchoring: Match must start exactly at currentIndex
        // 3. Length: Match must be non-empty
        // 4. Scope: Match must not exceed endIndex
        if (match.Success && match.Index == currentIndex && match.Length > 0 && (match.Index + match.Length <= endIndex))
        {
            int matchEndIndex = match.Index + match.Length;

            // 5. Boundary Check: End of scope OR followed by a boundary char
            bool endsAtBoundary = matchEndIndex == endIndex ||
                                 (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText[matchEndIndex]));

            if (endsAtBoundary)
            {
                HydratedNodeGraph hydratedRootNode = new(RootType, match, sourceText);
                tokenUnit = hydratedRootNode.Hydrate();

                return true;
            }
        }

        return false;
    }

    BuiltRegex BuildRegex()
    {
        RegexCollector collector = new();
        ComposeRegexLines(collector);



        return regexBuilder.GetBuiltRegex();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        if (RootType.IsAssignableTo(typeof(TokenUnitOneOf)))
            AlternatingComposer.Instance.Compose(builder, Children);
        else if (RootType.IsAssignableTo(typeof(TokenUnit)))
            ConcatenatingComposer.Instance.Compose(builder, Children);
    }

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        // todo: this represents a leak in our abstraction, because you can't (or perhaps shouldn't) try to 
        // get a value on a RootNode, but only of its children

        throw new NotImplementedException();
    }
}