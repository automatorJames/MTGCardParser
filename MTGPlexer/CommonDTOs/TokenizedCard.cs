namespace MTGPlexer.CommonDTOs;

public record TokenizedCard
(
    Card Card,
    List<TokenizedClause> Clauses
);

