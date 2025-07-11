namespace MTGPlexer.TokenAnalysis;

public class AggregateCardAnalysis
{
    public List<AnalyzedCard> AnalyzedCards = new();
    public Dictionary<TextSpan, UnmatchedSpanOccurrence> UnmatchedSegmentSpans { get; set; } = new(new TextSpanAsStringComparer());
    public Dictionary<Type, int> TokenCaptureCounts { get; set; } = new();
    public int TotalUnmatchedTokens { get; set; }

    public AggregateCardAnalysis(int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        var cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);
        var tokenUnitTypes = TokenClassRegistry.AppliedOrderTypes.OrderBy(t => t.Name).ToList();
        Analyze(cards);
    }

    public void Analyze(List<Card> cards)
    {
        foreach (var type in TokenClassRegistry.AppliedOrderTypes.OrderBy(x => x.Name))
            TokenCaptureCounts[type] = 0;

        foreach (Card card in cards)
        {
            var analyzedCard = new AnalyzedCard(card);
            analyzedCard.AddAccumulatedCapturedTokenTypeCounts(TokenCaptureCounts);

            TotalUnmatchedTokens += analyzedCard.UnmatchedTokenCount;

            foreach (var unmatchedSegmentSpan in analyzedCard.UnmatchedSegmentSpans)
                if (UnmatchedSegmentSpans.ContainsKey(unmatchedSegmentSpan))
                    UnmatchedSegmentSpans[unmatchedSegmentSpan]++;
                else
                    UnmatchedSegmentSpans[unmatchedSegmentSpan] = new UnmatchedSpanOccurrence(analyzedCard, unmatchedSegmentSpan);

            AnalyzedCards.Add(analyzedCard);
        }

        foreach (var card in AnalyzedCards)
            card.SetCapturedLines();
    }
}
