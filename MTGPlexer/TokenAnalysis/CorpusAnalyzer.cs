using MTGPlexer.TokenAnalysis.RegexDTOs;

namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// A consolidated processor that tokenizes a corpus of cards and produces a complete
/// analysis of both matched tokens (as SpanRoots) and word span trees in a single workflow.
/// </summary>
public class CorpusAnalyzer
{
    List<TokenUnit> _hydratedTokenUnits = [];

    /// <summary>
    /// Structured list of all processed cards, containing the hierarchical
    /// SpanRoot analysis for each line. This is the output for your matched-token logic.
    /// </summary>
    public List<ProcessedCard> ProcessedCards { get; }

    /// <summary>
    /// Word trees build around all maximal repeated spans across the corpus
    /// including TokenUnit class captures. Useful for analyzing which spans
    /// of text have not yet been captured by any TokenUnit.
    /// </summary>
    public DigestedSpanCorpus DigestedCorpusWithCaptureTokens { get; }

    /// <summary>
    /// Word trees build around all maximal repeated spans across the corpus
    /// without TokenUnit class captures. Useful for analyzing the original
    /// unaltered spans of a body of text to plan how to create ideal TokenUnits
    /// for maximally effective capture.
    /// </summary>
    public DigestedSpanCorpus DigestedCorpusOriginalText { get; }

    public TokenUnitCaptureSummary TopLevelTokenUnitCaptureSummary { get; }
    public TokenUnitCaptureSummary GlobalTokenUnitCaptureSummary { get; }

    public CorpusAnalyzer(List<Card> cards)
    {
        // Make a single pass through all cards and lines, performing all
        // initial processing (tokenization, SpanRoot generation, UnmatchedOccurrence collection).
        ProcessedCards = ProcessAllCards(cards, originalTextOnly: false);
        DigestedCorpusWithCaptureTokens = GetDigestedSpanCorpus(ProcessedCards);

        // Similarly initialize digested original corpus (no class tokens applied)
        var processedOriginalCards = ProcessAllCards(cards, originalTextOnly: true);
        DigestedCorpusOriginalText = GetDigestedSpanCorpus(processedOriginalCards);

        (TopLevelTokenUnitCaptureSummary, GlobalTokenUnitCaptureSummary) = TokenUnitCaptureSummary.CreateSummaries(_hydratedTokenUnits);
    }

    List<ProcessedCard> ProcessAllCards(List<Card> cards, bool originalTextOnly)
    {
        var processedCards = new List<ProcessedCard>();

        foreach (var card in cards)
        {
            var processedLines = new List<ProcessedLine>();
            for (int i = 0; i < card.CleanedLines.Length; i++)
            {
                var lineText = card.CleanedLines[i];
                if (string.IsNullOrWhiteSpace(lineText)) continue;

                var tokens = TokenTypeRegistry.TokenizeAndCoallesceUnmatched(lineText, originalTextOnly);

                // This single method call performs both analyses for the line.
                (var spanRoots, var spanOccurrences) = HydrateAndAnalyzeLine(card.Name, tokens, i);

                processedLines.Add(new ProcessedLine
                {
                    Card = card,
                    LineIndex = i,
                    SourceText = lineText,
                    SourceTokens = tokens,
                    SpanRoots = spanRoots,
                    SpanOccurrences = spanOccurrences
                });


            }
            processedCards.Add(new ProcessedCard { Card = card, Lines = processedLines });
        }

        return processedCards;
    }

    DigestedSpanCorpus GetDigestedSpanCorpus(List<ProcessedCard> processedCards)
    {
        var allUnmatchedOccurrences = processedCards
            .SelectMany(card => card.Lines)
            .SelectMany(line => line.SpanOccurrences)
            .ToList();

        return new(allUnmatchedOccurrences);
    }

    /// <summary>
    /// Process a list of tokens for a single line to derive SpanRoot hierarchies and SpanOccurrence records.
    /// </summary>
    (List<SpanRoot> spanRoots, List<SpanOccurrence> occurrences) HydrateAndAnalyzeLine(string cardName, List<Token<Type>> tokens, int lineIndex)
    {
        var roots = new List<SpanRoot>();
        var occurrences = new List<SpanOccurrence>();
        var tokenUnitCaptureSummaries = new List<TokenUnitCaptureSummary>();
        string textToPrecedeNext = null;
        var enclosingTokenCountPerType = new Dictionary<Type, int>();

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            var hydratedTokenUnit = TokenTypeRegistry.HydrateFromToken(token);
            _hydratedTokenUnits.Add(hydratedTokenUnit);

            // --- Analysis #1: Check for and record unmatched tokens ---
            // Create a new occurrence, giving it the context of the entire line's tokens.
            if (token.Kind == typeof(DefaultUnmatchedString))
                occurrences.Add(new SpanOccurrence(cardName, lineIndex, tokens, i));

            // --- Analysis #2: Build the SpanRoot hierarchy from the hydrated token ---
            var root = new SpanRoot(hydratedTokenUnit, cardName, textToPrecedeNext);
            textToPrecedeNext = null;

            if (root.Placement == TokenPlacement.FollowsPrevious)
                AttachRootTextToPreviousOrNext(root, isNext: false);
            else if (root.Placement == TokenPlacement.PrecedesNext)
                AttachRootTextToPreviousOrNext(root, isNext: true);
            else if (root.Placement == TokenPlacement.AlternatesFollowingAndPreceding)
            {
                enclosingTokenCountPerType.TryGetValue(hydratedTokenUnit.Type, out var currentCount);
                enclosingTokenCountPerType[hydratedTokenUnit.Type] = currentCount + 1;
                var isNext = (currentCount + 1) % 2 != 0;
                AttachRootTextToPreviousOrNext(root, isNext: isNext);
            }
            else
                roots.Add(root);
        }

        return (roots, occurrences);

        // Local helper for attaching text
        void AttachRootTextToPreviousOrNext(SpanBranch spanWithTextToAttach, bool isNext)
        {
            if (!isNext && !roots.Any())
                return;

            if (isNext)
                textToPrecedeNext = (textToPrecedeNext ?? "") + spanWithTextToAttach.Text;
            else
            {
                var appendedText = (roots[^1].AttachedFollowingText ?? "") + spanWithTextToAttach.Text;
                roots[^1] = roots[^1] with { AttachedFollowingText = appendedText };
            }
        }
    }
}