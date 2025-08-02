using MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

namespace MTGPlexer.DTOs;

/// <summary>
/// Represents a single card and all its processed lines of text.
/// This is a lightweight container for the results of the CorpusAnalyzer.
/// </summary>
public record ProcessedCard
{
    public Card Card { get; init; }
    public List<ProcessedLine> Lines { get; init; }

    public List<UnmatchedSpanOccurrence> UnmatchedSpans =>
        Lines
        .SelectMany(x => x.UnmatchedOccurrences)
        .ToList();

    public bool IsFullyMatched => UnmatchedSpans.Count() == 0;
}