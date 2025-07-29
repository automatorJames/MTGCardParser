namespace MTGPlexer.TokenAnalysis;

public record CardDigester
{
    public List<CardDigest> DigestedCards { get; }
    public Dictionary<Type, int> TokenCounts { get; } = [];
    public List<UnmatchedSpanContext> UnmatchedSpanContexts { get; }

    public CardDigester(List<Card> cards)
    {
        DigestedCards = cards.Select(x => new CardDigest(x)).ToList();
        DigestedCards.ForEach(x => TokenCounts.CombineDictionaryCounts(x.TokenCounts));
        UnmatchedSpanContexts = GetUnmatchedSpanContexts(DigestedCards);
    }

    public static List<UnmatchedSpanContext> GetUnmatchedSpanContexts(List<CardDigest> digestedCards)
    {
        // 1) get global span counts
        var unmatchedSpanCounts = SpanOccurrenceCounter.GetUnmatchedSpanCounts(digestedCards);

        // 2) flatten every line of every card into (CardName, Words[]) tuples
        var tokenizedLines = digestedCards
            .SelectMany(cd => cd.Lines
                .Select(line => (
                    CardName: cd.Card.Name,
                    Words: line.SourceText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                ))
            )
            .ToList();

        var result = new List<UnmatchedSpanContext>(unmatchedSpanCounts.Count);

        foreach (var span in unmatchedSpanCounts)
        {
            // break the span into its words
            var spanWords = span.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var prevFreq = new Dictionary<string, int>(StringComparer.Ordinal);
            var nextFreq = new Dictionary<string, int>(StringComparer.Ordinal);
            var contexts = new List<SpanContext>();

            // 3) scan each tokenized line for matches
            foreach (var line in tokenizedLines)
            {
                var words = line.Words;
                for (int i = 0; i + spanWords.Length <= words.Length; i++)
                {
                    // fast check for the span
                    bool match = true;
                    for (int j = 0; j < spanWords.Length; j++)
                    {
                        if (!string.Equals(words[i + j], spanWords[j], StringComparison.Ordinal))
                        {
                            match = false;
                            break;
                        }
                    }
                    if (!match)
                        continue;

                    // record one preceding word
                    if (i > 0)
                    {
                        var w = words[i - 1];
                        prevFreq[w] = prevFreq.GetValueOrDefault(w) + 1;
                    }

                    // record one following word
                    int after = i + spanWords.Length;
                    if (after < words.Length)
                    {
                        var w = words[after];
                        nextFreq[w] = nextFreq.GetValueOrDefault(w) + 1;
                    }

                    // build the "up to 5 words before…span…5 words after" context
                    int start = Math.Max(0, i - 5);
                    int end = Math.Min(words.Length, i + spanWords.Length + 5);
                    var window = words[start..end];
                    var ctxText = string.Join(' ', window);

                    //contexts.Add(new SpanContext(
                    //    CardName: line.CardName,
                    //    Context: ctxText
                    //));
                }
            }

            // 4) sort adjacent‑word lists by frequency desc
            var preceding = prevFreq
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new SpanAdjacentWord(kv.Key, kv.Value))
                .ToList();

            var following = nextFreq
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new SpanAdjacentWord(kv.Key, kv.Value))
                .ToList();

            // 5) build final record
            result.Add(new UnmatchedSpanContext(
                UnmatchedSpanCount: span,
                Preceding: preceding,
                Following: following,
                Contexts: contexts
            ));
        }

        return result;
    }
}