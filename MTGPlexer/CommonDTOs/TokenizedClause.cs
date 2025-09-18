namespace MTGPlexer.CommonDTOs;

public record TokenizedClause
(
    List<TokenUnit> Tokens,
    int ClauseIndex,
    string OriginalText
);