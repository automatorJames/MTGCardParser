using System.Diagnostics;

namespace MTGPlexer.TokenAnalysisDTOs;

/// <summary>
/// A consolidated processor that tokenizes a corpus of cards and produces a complete
/// analysis of both matched tokens (as TokenCaptureSummary) and word span trees in a single workflow.
/// </summary>
public class CorpusAnalyzer
{
    List<TokenUnit> _hydratedTokenUnits = [];

    /// <summary>
    /// Structured list of all processed cards, containing the hierarchical
    /// TokenCaptureSummary analysis for each line. This is the output for your matched-token logic.
    /// </summary>
    public List<ProcessedCard> ProcessedCards { get; }

    /// <summary>
    /// Word trees build around all maximal repeated spans across the corpus
    /// including TokenUnit class captures. Useful for analyzing which spans
    /// of text have not yet been captured by any TokenUnit.
    /// </summary>
    public DigestedTextCorpus DigestedCorpusWithCaptureTokens { get; }

    /// <summary>
    /// Word trees build around all maximal repeated spans across the corpus
    /// without TokenUnit class captures. Useful for analyzing the original
    /// unaltered spans of a body of text to plan how to create ideal TokenUnits
    /// for maximally effective capture.
    /// </summary>
    public DigestedTextCorpus DigestedCorpusOriginalText { get; }

    public TokenUnitCaptureSummary TokenCaptureSummary { get; }

    public CorpusAnalyzer(List<Card> cards)
    {
        // Make a single pass through all cards and lines, performing all
        // initial processing (tokenization, TokenCaptureSummary generation, UnmatchedOccurrence collection).
        ProcessedCards = ProcessAllCards(cards, originalTextOnly: false);
        DigestedCorpusWithCaptureTokens = GetDigestedSpanCorpus(ProcessedCards);

        // Similarly initialize digested original corpus (no class tokens applied)
        var processedOriginalCards = ProcessAllCards(cards, originalTextOnly: true);
        DigestedCorpusOriginalText = GetDigestedSpanCorpus(processedOriginalCards);

        TokenCaptureSummary = new TokenUnitCaptureSummary(_hydratedTokenUnits);
    }

    List<ProcessedCard> ProcessAllCards(List<Card> cards, bool originalTextOnly)
    {
        var processedCards = new List<ProcessedCard>();

        foreach (var card in cards)
        {

            var processedLines = new List<ProcessedLine>();
            for (int i = 0; i < card.FormattedLinesLower.Length; i++)
            {
                var lineText = card.FormattedLinesLower[i];
                var originalLineText = card.FormattedLines[i];

                if (string.IsNullOrWhiteSpace(lineText))
                    continue;

                var lineTokens = TokenTypeRegistry.Tokenize(lineText, originalTextOnly);

                _hydratedTokenUnits.AddRange(lineTokens);

                var unmatchedStringOccurrences = GetUnmatchedStringOccurrences(card, lineTokens, i, originalLineText);
                var analyzedCard = TokenTypeRegistry.CardTokenizer.GetCardTokenAnalysis(card);
                var analysisRoots = analyzedCard.Where(x => x.ClauseIndex == i).ToList();

                processedLines.Add(new ProcessedLine
                {
                    Card = card,
                    LineIndex = i,
                    EvaluatedText = lineText,
                    SourceTokens = lineTokens,
                    TokenAnalysisRoots = analysisRoots,
                    SpanOccurrences = unmatchedStringOccurrences
                });
            }
            processedCards.Add(new ProcessedCard { Card = card, Lines = processedLines });
        }

        return processedCards;
    }

    DigestedTextCorpus GetDigestedSpanCorpus(List<ProcessedCard> processedCards)
    {
        var allUnmatchedOccurrences = processedCards
            .SelectMany(card => card.Lines)
            .SelectMany(line => line.SpanOccurrences)
            .ToList();

        return new(allUnmatchedOccurrences);
    }

    List<UnmatchedTextOccurrence> GetUnmatchedStringOccurrences(Card card, List<TokenUnit> lineTokens, int lineIndex, string originalLineText)
    {
        var occurrences = new List<UnmatchedTextOccurrence>();

        for (int i = 0; i < lineTokens.Count; i++)
        {
            var token = lineTokens[i];

            // --- Analysis #1: Check for and record unmatched tokens ---
            // Create a new occurrence, giving it the context of the entire line's tokens.
            if (token.Type == typeof(DefaultUnmatchedString))
                occurrences.Add(new UnmatchedTextOccurrence(card.Name, lineIndex, lineTokens, i));
        }

        return occurrences;
    }
}