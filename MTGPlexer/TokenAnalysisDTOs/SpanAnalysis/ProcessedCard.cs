namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

using MTGPlexer.Colors;

/// <summary>
/// Represents a single card and all its processed lines of text.
/// This is a lightweight container for the results of the CorpusAnalyzer.
/// </summary>
public class ProcessedCard
{
    static HashSet<string> _irrelevantUnmatchedStrings = [".", ". ", " "];

    public Card Card { get; init; }
    public List<ProcessedLine> Lines { get; init; }
    public bool IsFullyMatched { get; init; }

    public ProcessedCard(Card card)
    {
        Card = card;
        Lines = ProcessedLine.GetAll(card);

        // "Fully matched" means no unmatched text occurrences (except isolated periods) exist
        IsFullyMatched = Lines
            .SelectMany(x => x.UnmatchedTextOccurrences.Where(y => !_irrelevantUnmatchedStrings.Contains(y.Text)))
            .Count() == 0;
    }
}