namespace MTGPlexer.TokenAnalysis;

public class UnmatchedSpanOccurrence
{
    public int Count { get; set; }
    public AnalyzedCard FirstRepresentativeCard { get; set; }
    public TextSpan FirstRepresentativeCardOccurrence { get; set; }

    public UnmatchedSpanOccurrence(AnalyzedCard firstRepresentativeCard, TextSpan firstRepresentativeCardOccurrence)
    {
        Count = 1;
        FirstRepresentativeCard = firstRepresentativeCard;
        FirstRepresentativeCardOccurrence = firstRepresentativeCardOccurrence;
    }

    public static UnmatchedSpanOccurrence operator ++(UnmatchedSpanOccurrence x)
    {
        x.Count++;
        return x;
    }
}