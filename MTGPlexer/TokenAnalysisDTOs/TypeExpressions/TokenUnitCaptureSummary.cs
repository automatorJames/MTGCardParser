namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

/// <summary>
/// A summary of all property values captured for a given set of TokenUnits,
/// organized by Type, then by property path, with values ordered by frequency.
/// </summary>
public class TokenUnitCaptureSummary
{
    public Dictionary<Type, TokenUnitCapture> TokenUnitCaptures { get; } = [];
    public Dictionary<Type, TokenUnitCapture> UnmatchedTokenUnits { get; } = [];

    public TokenUnitCaptureSummary(List<ProcessedCard> processedCards)
    {
        var allTokenUnits = processedCards
            .SelectMany(x => x.Lines)
            .SelectMany(x => x.SpanRoots)
            .Select(x => x.RootToken)
            .Where(x => x is not DefaultUnmatchedString);

        var orderedRootTokenUnitTypes = TokenTypeRegistry.Templates.Keys
            .OrderBy(TokenTypeRegistry.AppliedOrderTypes.IndexOf);

        foreach (var rootType in orderedRootTokenUnitTypes)
        {
            var rootTokensOfType = allTokenUnits.Where(x => x.Type == rootType).ToList();

            if (!rootTokensOfType.Any())
                continue;

            TokenUnitCaptures[rootType] = new(rootType, rootTokensOfType);
        }

        // Make TokenUniCaptures for types that have no captures in case GUI callers want to see their regex patterns
        foreach (var type in orderedRootTokenUnitTypes.Except(TokenUnitCaptures.Keys).Where(x => x != typeof(DefaultUnmatchedString)))
            UnmatchedTokenUnits[type] = new TokenUnitCapture(type);
    }
}