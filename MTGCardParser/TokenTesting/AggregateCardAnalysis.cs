namespace MTGCardParser.TokenTesting;

public class AggregateCardAnalysis
{
    public List<CardAnalysis> AnalyzedCards = new();
    public Dictionary<TextSpan, UnmatchedSpanOccurrence> UnmatchedSegmentSpans { get; set; } = new(new TextSpanAsStringComparer());
    public Dictionary<Type, int> TokenCaptureCounts { get; set; } = new();
    public int TotalUnmatchedTokens { get; set; }

    public AggregateCardAnalysis(List<Card> cards, bool hydrateAllTokenInstances = true)
    {
        foreach (var type in TokenClassRegistry.AppliedOrderTypes.OrderBy(x => x.Name))
            TokenCaptureCounts[type] = 0;

        foreach (Card card in cards)
        {
            var analyzedCard = new CardAnalysis(card);
            analyzedCard.AddAccumulatedCapturedTokenTypeCounts(TokenCaptureCounts);

            TotalUnmatchedTokens += analyzedCard.UnmatchedTokenCount;

            foreach (var unmatchedSegmentSpan in analyzedCard.UnmatchedSegmentSpans)
                if (UnmatchedSegmentSpans.ContainsKey(unmatchedSegmentSpan))
                    UnmatchedSegmentSpans[unmatchedSegmentSpan]++;
                else
                    UnmatchedSegmentSpans[unmatchedSegmentSpan] = new UnmatchedSpanOccurrence(analyzedCard, unmatchedSegmentSpan);

            AnalyzedCards.Add(analyzedCard);
        }

        if (hydrateAllTokenInstances)
            foreach (var card in AnalyzedCards)
                card.SetClauseEffects();
    }
}

