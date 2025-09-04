namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

/// <summary>
/// Base record for the composite pattern representing a piece of a regex.
/// </summary>
public abstract record RegexFragment
{
    // A reference to the parent can be useful for upward traversal if needed later.
    public RegexGroupFragment Parent { get; internal set; }
}