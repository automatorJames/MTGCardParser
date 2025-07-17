namespace MTGPlexer.TokenAnalysis;

public record CardDigester
{
    public List<CardDigest> DigestedCards { get; }
    public Dictionary<Type, int> TokenCounts { get; } = [];

    public CardDigester(int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        var cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);
        DigestedCards = cards.Select(x => new CardDigest(x)).ToList();
        DigestedCards.ForEach(x => TokenCounts.CombineDictionaryCounts(x.TokenCounts));
    }
}

