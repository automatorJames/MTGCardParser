namespace MTGPlexer.TokenAnalysis.DTOs;

public record CardDigest
{
    public Card Card { get; }
    public List<CardLine> Lines { get; } = [];
    public List<string> UnmatchedSpans =>
        Lines
        .SelectMany(x => x.UnmatchedSpans)
        .Distinct()
        .ToList();
    public bool IsFullyMatched => UnmatchedSpans.Count() == 0;  

    public CardDigest(Card card)
    {
        for (int i = 0; i < card.CleanedLines.Length; i++)
        {
            Card = card;
            var line = card.CleanedLines[i];
            var lineTokens = TokenTypeRegistry.Tokenizer.Tokenize(line).ToList();
            Lines.Add(new(card, lineTokens, i));
        }
    }
}

