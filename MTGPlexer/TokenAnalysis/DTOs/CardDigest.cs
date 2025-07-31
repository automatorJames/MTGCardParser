namespace MTGPlexer.TokenAnalysis.DTOs;

public record CardDigest
{
    public Card Card { get; }
    public List<CardLine> Lines { get; } = [];
    public List<SpanContext> UnmatchedSpans =>
        Lines
        .SelectMany(x => x.UnmatchedSpans)
        .ToList();

    public bool IsFullyMatched => UnmatchedSpans.Count() == 0;

    public Dictionary<Type, int> TokenCounts { get; } = [];

    public CardDigest(Card card)
    {
        for (int i = 0; i < card.CleanedLines.Length; i++)
        {
            Card = card;
            var line = card.CleanedLines[i];
            var lineTokens = TokenTypeRegistry.TokenizeAndCoallesceUnmatched(line);
            Lines.Add(new(card, line, lineTokens, i));
            CountTokenTypes(lineTokens);
        }
    }

    void CountTokenTypes(List<Token<Type>> tokens)
    {
        foreach (var token in tokens)
        {
            if (!TokenCounts.ContainsKey(token.Kind))
                TokenCounts[token.Kind] = 0;

            TokenCounts[token.Kind]++;
        }
    }
}

