namespace MTGPlexer;

/// <summary>
/// Represents a single tokenized line or clause from a card.
/// </summary>
public record TokenizedClause
(
    IReadOnlyList<TokenUnit> Tokens,
    int ClauseIndex,
    string OriginalText,
    string PathPrefix
);