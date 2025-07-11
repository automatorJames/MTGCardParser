namespace MTGPlexer.TokenAnalysis;

public class AnalyzedCard
{
    public Card Card { get; set; }

    public List<List<Token<Type>>> ProcessedLineTokens { get; private set; } = new();
    public List<Token<Type>> CombinedTokens => ProcessedLineTokens.SelectMany(x => x).ToList();
    public List<Token<Type>[]> UnmatchedSegments { get; private set; } = new();
    public List<TextSpan> UnmatchedSegmentSpans { get; private set; } = new();
    public List<string> UnmatchedSegmentCombinations { get; private set; } = new();
    public List<CapturedLine> CapturedLines { get; private set; } = new();

    public int UnmatchedTokenCount => UnmatchedSegments
        .SelectMany(x => x)
        .Where(x => x.Kind == typeof(DefaultUnmatchedString))
        .Count();

    public AnalyzedCard(Card card)
    {
         Card = card;

        foreach (var line in card.CleanedLines)
        {
            var lineTokens = TokenClassRegistry.Tokenizer.Tokenize(line).ToList();
            ProcessedLineTokens.Add(lineTokens);
        }

        SetUnmatchedSegments(ignoreSingleWordSegments: false);
        SetUnmatchedSegmentSpans();
        SetUnmatchedSegmentCombinations();
    }

    void SetUnmatchedSegments(bool ignoreSingleWordSegments)
    {
        int minimumSegmentLength = ignoreSingleWordSegments ? 2 : 1;

        // holds result
        UnmatchedSegments = new();

        // holds working buffer
        List<Token<Type>> tokens = new();

        foreach (var lineTokenSet in ProcessedLineTokens)
        {
            for (int i = 0; i < lineTokenSet.Count; i++)
            {
                var token = lineTokenSet[i];

                // token strings represent unmatched tokens
                // although Punctuation tokens are technically captured, we want to treat them as uncaptured and later append them to the preceding string
                if (token.Kind == typeof(DefaultUnmatchedString))
                    tokens.Add(token);

                // Add the accumulated tokens to UnmatchedSegments
                else
                    FlushWordBuffer();
            }

            FlushWordBuffer();
        }

        void FlushWordBuffer()
        {
            if (tokens.Count >= minimumSegmentLength)
                UnmatchedSegments.Add(tokens.ToArray());

            tokens = new();
        }
    }
    void SetUnmatchedSegmentSpans()
    {
        UnmatchedSegmentSpans = new();

        foreach (var segment in UnmatchedSegments)
        {
            var startSpan = segment.First().Span;
            var endSpan = segment.Last().Span;
            var combinedSpan = new TextSpan(
                startSpan.Source,
                startSpan.Position,
                endSpan.Position.Absolute + endSpan.Length - startSpan.Position.Absolute
            );

            UnmatchedSegmentSpans.Add(combinedSpan);
        }
    }

    void SetUnmatchedSegmentCombinations()
    {
        UnmatchedSegmentCombinations = new();

        foreach (var segment in UnmatchedSegments)
        {
            for (int startOffset = 0; startOffset < segment.Length; startOffset++)
                for (int length = segment.Length - startOffset; length >= 2; length--)
                    UnmatchedSegmentCombinations.Add(string.Join(' ', segment.Skip(startOffset).Take(length)));
        }
    }

    public List<string> GetAllSegmentCombinations()
    {
        List<string> combinations = new();

        foreach (var line in Card.Text.Split("\n"))
        {
            var wordsArray = line.Split(' ');

            for (int startOffset = 0; startOffset < wordsArray.Length; startOffset++)
                for (int length = wordsArray.Length - startOffset; length >= 2; length--)
                    combinations.Add(string.Join(' ', wordsArray.Skip(startOffset).Take(length)));
        }

        return combinations;
    }

    public void AddAccumulatedCapturedTokenTypeCounts(Dictionary<Type, int> dict)
    {
        var nonPlaceholderNonStringTokens = CombinedTokens.Where(x => x.Kind != typeof(DefaultUnmatchedString));

        foreach (var token in nonPlaceholderNonStringTokens)
            if (!dict.ContainsKey(token.Kind))
                throw new Exception($"Dictionary contains no key for {token.Kind.Name}");
            else
                dict[token.Kind]++;
    }

    public void SetCapturedLines()
    {
        for (int i = 0; i < ProcessedLineTokens.Count; i++)
        {
            List<Token<Type>> line = ProcessedLineTokens[i];
            List<TokenUnit> list = new();

            foreach (var token in line)
                list.Add(TokenClassRegistry.HydrateFromToken(token));

            CapturedLines.Add(new (list, Card, i));
        }
    }

    public override string ToString() => Card.ToString();
}

