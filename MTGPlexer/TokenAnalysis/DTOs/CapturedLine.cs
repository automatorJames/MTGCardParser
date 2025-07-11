namespace MTGPlexer.TokenAnalysis.DTOs;

public record CapturedLine
{
    public List<PositionalToken> Tokens { get; init; }
    public int LineIndex { get; init; }

    public CapturedLine(List<TokenUnit> orderedTokens, Card card, int lineIndex)
    {
        Tokens = orderedTokens.Select((x, idx) => new PositionalToken(x, card, lineIndex, idx)).ToList();
        LineIndex = lineIndex;
    }
}

