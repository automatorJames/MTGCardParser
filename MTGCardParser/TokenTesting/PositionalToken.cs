namespace MTGCardParser.TokenTesting;

public record PositionalToken
{
    public TokenUnit Token { get; init; }
    public int Index { get; init; }
    public string CaptureId { get; init; }
    public List<PositionalToken> Children { get; init; } = [];

    public PositionalToken(TokenUnit token, Card card, int lineIndex, int tokenIndex, int? childIndex = null)
    {
        Index = tokenIndex;
        Token = token;
        CaptureId = $"{card.Name}-{card.CardId}-{lineIndex}-{tokenIndex}";

        if (childIndex.HasValue)
            CaptureId += $"-child{childIndex.Value}";

        foreach (var (child, idx) in token.ChildTokens.OrderBy(c => c.MatchSpan.Position.Absolute).Select((token, index) => (token, index)))
            Children.Add(new (child, card, lineIndex, tokenIndex, idx));
    }
}

