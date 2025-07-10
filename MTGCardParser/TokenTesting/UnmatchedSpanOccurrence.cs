namespace MTGCardParser.TokenTesting;

public class UnmatchedSpanOccurrence
{
    public int Count { get; set; }
    public CardAnalysis FirstRepresentativeCard { get; set; }
    public TextSpan FirstRepresentativeCardOccurrence { get; set; }

    public UnmatchedSpanOccurrence(CardAnalysis firstRepresentativeCard, TextSpan firstRepresentativeCardOccurrence)
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