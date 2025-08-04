namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// A consolidated processor that tokenizes a corpus of cards and produces a complete
/// analysis of both matched tokens (as SpanRoots) and unmatched spans in a single workflow.
/// </summary>
public class CorpusAnalyzer
{
    /// <summary>
    /// A structured list of all processed cards, containing the hierarchical
    /// SpanRoot analysis for each line. This is the output for your matched-token logic.
    /// </summary>
    public List<ProcessedCard> ProcessedCards { get; }

    public UnmatchedDigestedCorpus UnmatchedDigestedCorpus { get; }

    /// <summary>
    /// A global count of all token types across the entire corpus.
    /// </summary>
    public Dictionary<Type, int> GlobalTokenCounts { get; } = [];

    public CorpusAnalyzer(List<Card> cards)
    {
        cards.ForEach(x => TokenTypeRegistry.CorpusItemPalettes[x.Name] = new DeterministicPalette(x.Name));

        // STEP 1: Make a single pass through all cards and lines, performing all
        // initial processing (tokenization, SpanRoot generation, UnmatchedOccurrence collection).
        ProcessedCards = ProcessAllCards(cards);

        // STEP 2: Collect all unmatched occurrences from the processed data.
        var allUnmatchedOccurrences = ProcessedCards
            .SelectMany(card => card.Lines)
            .SelectMany(line => line.UnmatchedOccurrences)
            .ToList();

        UnmatchedDigestedCorpus = new(allUnmatchedOccurrences);
    }

    private List<ProcessedCard> ProcessAllCards(List<Card> cards)
    {
        var processedCards = new List<ProcessedCard>();

        foreach (var card in cards)
        {
            var processedLines = new List<ProcessedLine>();
            for (int i = 0; i < card.CleanedLines.Length; i++)
            {
                var lineText = card.CleanedLines[i];
                if (string.IsNullOrWhiteSpace(lineText)) continue;

                var tokens = TokenTypeRegistry.TokenizeAndCoallesceUnmatched(lineText);
                CountTokenTypes(tokens);

                // This single method call performs both analyses for the line.
                (var spanRoots, var unmatchedSpanOccurrences) = HydrateAndAnalyzeLine(card.Name, tokens, i);

                processedLines.Add(new ProcessedLine
                {
                    Card = card,
                    LineIndex = i,
                    SourceText = lineText,
                    SourceTokens = tokens,
                    SpanRoots = spanRoots,
                    UnmatchedOccurrences = unmatchedSpanOccurrences
                });
            }
            processedCards.Add(new ProcessedCard { Card = card, Lines = processedLines });
        }

        return processedCards;
    }

    /// <summary>
    /// Process a list of tokens for a single line to produce both the SpanRoot hierarchy and the list of UnmatchedSpanOccurrence records.
    /// </summary>
    private (List<SpanRoot> spanRoots, List<UnmatchedSpanOccurrence> occurrences) HydrateAndAnalyzeLine(string cardName, List<Token<Type>> tokens, int lineIndex)
    {
        var roots = new List<SpanRoot>();
        var occurrences = new List<UnmatchedSpanOccurrence>();
        string textToPrecedeNext = null;
        var enclosingTokenCountPerType = new Dictionary<Type, int>();

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            // --- Analysis #1: Check for and record unmatched tokens ---
            // Create a new occurrence, giving it the context of the entire line's tokens.
            if (token.Kind == typeof(DefaultUnmatchedString))
                occurrences.Add(new UnmatchedSpanOccurrence(cardName, lineIndex, tokens, i));

            // --- Analysis #2: Hydrate and build the SpanRoot hierarchy ---
            var hydratedTokenUnit = TokenTypeRegistry.HydrateFromToken(token);
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
            {
                roots.Add(root);
            }
        }

        return (roots, occurrences);

        // Local helper for attaching text, as in your original code.
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

    private void CountTokenTypes(List<Token<Type>> tokens)
    {
        foreach (var token in tokens)
        {
            GlobalTokenCounts.TryGetValue(token.Kind, out var currentCount);
            GlobalTokenCounts[token.Kind] = currentCount + 1;
        }
    }
}