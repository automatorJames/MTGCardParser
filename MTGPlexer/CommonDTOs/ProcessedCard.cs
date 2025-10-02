namespace MTGPlexer.CommonDTOs;

/// <summary>
/// Represents a single card and all its processed lines of text.
/// This is a lightweight container for the results of the CorpusAnalyzer.
/// </summary>
public class ProcessedCard
{
    public Card Card { get; init; }
    public List<ProcessedLine> Lines { get; init; }
    public bool IsFullyMatched { get; init; }

    public ProcessedCard(Card card)
    {
        Card = card;
        Lines = ProcessedLine.GetAll(card);
        IsFullyMatched = Lines.SelectMany(x => x.UnmatchedTextOccurrences).Count() == 0;
    }
}