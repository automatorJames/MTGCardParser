namespace MTGPlexer.RegexGeneration.Graph;

public class RegexGraph
{
    static readonly char[] _boundaryChars = [' ', '.'];

    public Type RootTokenUnitType { get; }
    public TokenUnitNode RootNode { get; }
    public BuiltRegex BuiltRegex { get; }

    public RegexGraph(Type rootTokenUnitType, TokenUnitNode rootNode)
    {
        RootTokenUnitType = rootTokenUnitType;
        RootNode = rootNode;
        RegexCollector collector = new();
        RootNode.AppendRegexBricks(collector);
        BuiltRegex = collector.GetBuiltRegex();
    }

    public static RegexGraph Create(Type rootTokenUnitType)
    {
        TypeNavigation navigation = new(rootTokenUnitType);

        TokenUnitNode contentRoot = rootTokenUnitType switch
        {
            { } t when t.IsAssignableTo(typeof(TokenUnitCompound)) => new TokenUnitCompoundNode(null, navigation),
            { } t when t.IsAssignableTo(typeof(TokenUnitOneOf)) => new TokenUnitOneOfNode(null, navigation),
            { } t when t.IsAssignableTo(typeof(DefaultUnmatchedString)) => new UnmatchedTokenUnitNode(null, navigation),
            { } t when t.IsAssignableTo(typeof(TokenUnit)) =>new TokenUnitNode(null, navigation),
            _ => throw new ArgumentException($"Type '{rootTokenUnitType.Name}' is not a {nameof(TokenUnit)} type")
        };

        return new(rootTokenUnitType, contentRoot);
    }

    public bool TryMatch(string sourceText, out TokenUnit tokenUnit) => 
        TryMatch(sourceText, 0, sourceText.Length, out tokenUnit);

    /// <summary>
    /// Evaluates if the source text at the current index satisfies the regex and MTG boundary rules.
    /// </summary>
    public bool TryMatch(string sourceText, int currentIndex, int endIndex, out TokenUnit tokenUnit)
    {
        //tokenUnit = null;
        //var match = BuiltRegex.Regex.Match(sourceText, currentIndex);
        //
        //// 1. Regex Success
        //// 2. Anchoring: Match must start exactly at currentIndex
        //// 3. Length: Match must be non-empty
        //// 4. Scope: Match must not exceed endIndex
        //if (match.Success && match.Index == currentIndex && match.Length > 0 && (match.Index + match.Length <= endIndex))
        //{
        //    int matchEndIndex = match.Index + match.Length;
        //
        //    // 5. Boundary Check: End of scope OR followed by a boundary char
        //    bool endsAtBoundary = matchEndIndex == endIndex ||
        //                         (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText[matchEndIndex]));
        //
        //    if (endsAtBoundary)
        //    {
        //        HydratedNodeGraph hydratedRootNode = new(RootTokenUnitType, match, sourceText);
        //        tokenUnit = hydratedRootNode.Hydrate();
        //
        //        return true;
        //    }
        //}

        tokenUnit = null;
        return false;
    }

    //public TokenUnit Hydrate(Match match, string sourceText)
    //{
    //    var captureContext = CaptureContext.Create(match, sourceText);
    //    var instance = (TokenUnit)Activator.CreateInstance(RootTokenUnitType);
    //    
    //    foreach (var branchChild in RootNode.Children.OfType<NamedGroupNode>())
    //    {
    //        // will return false only if an underlying property has AbortIfSetPropertyToNull == true
    //        // and the property value is null
    //        var setSuccessfully = branchChild.SetPropertyValue(captureContext, instance);
    //    
    //        if (!setSuccessfully)
    //            return null;
    //    }
    //    
    //    instance.NodeGraph = this;
    //    
    //    return instance;
    //}
}
