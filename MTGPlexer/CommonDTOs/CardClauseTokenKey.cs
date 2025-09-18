namespace MTGPlexer.CommonDTOs;

public record CardClauseTokenKey
(
    string CardName,
    string TokenPropPath,
    int ClauseIndex,
    int CaptureStart,
    int CaptureEnd
)
{
    public string Key = $"CardName-cl{ClauseIndex}-{TokenPropPath}";
}

