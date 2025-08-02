namespace MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

public record UnmatchedSpanCount
{
    public string Text { get; }
    public int OccurrenceCount { get; }
    public int Length { get; }
    public int WordCount { get; }

    public UnmatchedSpanCount(string text, int occurrenceCount)
    {
        Text = text;
        OccurrenceCount = occurrenceCount;
        Length = text.Length;
        WordCount = text.Split(' ').Count();
    }
}