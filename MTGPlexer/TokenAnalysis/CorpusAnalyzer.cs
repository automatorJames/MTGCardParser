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

    /// <summary>
    /// The final, consolidated list of all unique unmatched spans (full and maximal sub-spans)
    /// found in the corpus. This is the output for your unmatched-text analysis.
    /// </summary>
    public List<AnalyzedUnmatchedSpan> AnalyzedUnmatchedSpans { get; }

    /// <summary>
    /// A global count of all token types across the entire corpus.
    /// </summary>
    public Dictionary<Type, int> GlobalTokenCounts { get; } = [];

    public CorpusAnalyzer(List<Card> cards)
    {
        // STEP 1: Make a single pass through all cards and lines, performing all
        // initial processing (tokenization, SpanRoot generation, UnmatchedOccurrence collection).
        ProcessedCards = ProcessAllCards(cards);

        // STEP 2: Collect all unmatched occurrences from the processed data.
        var allUnmatchedOccurrences = ProcessedCards
            .SelectMany(card => card.Lines)
            .SelectMany(line => line.UnmatchedOccurrences)
            .ToList();

        // STEP 3: Use the automaton on the collected occurrences to find all maximal and full spans.
        var spanCounts = FindAllMaximalSpans(allUnmatchedOccurrences);

        // STEP 4: Consolidate the automaton results into the final rich output format.
        AnalyzedUnmatchedSpans = ConsolidateResults(spanCounts, allUnmatchedOccurrences);
    }

    private List<ProcessedCard> ProcessAllCards(List<Card> cards)
    {
        cards = new List<Card>
        {
            new Card { Name = "1", Text = "The dog runs fast and licks faces with glee" },
            new Card { Name = "2", Text = "The cat runs fast and licks milk with glee" },
            new Card { Name = "3", Text = "The llama runs fast and licks milk while shitting" }
        };

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
    /// This helper method integrates the logic from your original GetHydratedTokenUnits.
    /// It processes a list of tokens for a single line to produce both the SpanRoot
    /// hierarchy and the list of UnmatchedSpanOccurrence records.
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
            if (token.Kind == typeof(DefaultUnmatchedString))
            {
                // Create a new occurrence, giving it the context of the entire line's tokens.
                occurrences.Add(new UnmatchedSpanOccurrence(cardName, lineIndex, tokens, i));
            }

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
            if (!isNext && !roots.Any()) return;

            if (isNext)
            {
                textToPrecedeNext = (textToPrecedeNext ?? "") + spanWithTextToAttach.Text;
            }
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

    #region Suffix Automaton Logic and Result Consolidation

    /// <summary>
    /// Represents a single state (or node) in a suffix automaton. Each state corresponds to
    /// one or more substrings (spans of words) that end at the same positions in the text.
    /// </summary>
    private class AutomatonState
    {
        /// <summary>
        /// An array of transitions to other states. The index of the array corresponds to a
        /// word's unique ID, and the value at that index is the ID of the state reached by
        /// following that word. A value of -1 indicates no transition exists for that word.
        /// This represents all the "next words" the span can be extended with in the corpus.
        /// </summary>
        public int[] Next;

        /// <summary>
        /// The 'suffix link'. It points to the index of the state that represents the longest
        /// proper suffix of the string represented by this state (i.e. the next-shortest sub-string).
        /// A value of -1 indicates no link (used only by the initial state).
        /// </summary>
        public int Link;

        /// <summary>
        /// The length of the longest substring (span of words) that ends in this state.
        /// </summary>
        public int Length;

        /// <summary>
        /// The number of times the substrings corresponding to this state appear in the input text.
        /// This value is calculated only after the automaton is fully built.
        /// </summary>
        public long Count;

        /// <summary>
        /// The end position (index in the flattened word list) of the *first* occurrence
        /// of the longest substring ending at this state.
        /// </summary>
        public int FirstOccurrenceEndPosition;

        /// <summary>
        /// Initializes a new instance of the AutomatonState class.
        /// </summary>
        /// <param name="alphabetSize">The total number of unique words plus one (for the separator),
        /// used to initialize the size of the transitions array.</param>
        public AutomatonState(int alphabetSize)
        {
            Next = new int[alphabetSize];
            // Pre-fill the transitions array with -1 to signify that no paths exist yet.
            Array.Fill(Next, -1);
            Link = -1;
        }
    }

    private static Dictionary<string, int> FindAllMaximalSpans(List<UnmatchedSpanOccurrence> allOccurrences)
    {
        // 1) Build a word→ID map
        var wordToId = new Dictionary<string, int>(StringComparer.Ordinal);
        var idToWord = new List<string> { null }; // index 0 is separator
        int nextId = 1;

        // holds sequences of word arrays converted to int arrays, where each int is a unique id
        var sequences = new List<int[]>();
        foreach (var occurrence in allOccurrences)
        {
            var seq = new int[occurrence.SpanWordCount];
            for (int i = 0; i < occurrence.SpanWordCount; i++)
            {
                if (!wordToId.TryGetValue(occurrence.SpanWords[i], out var id))
                {
                    id = nextId++;
                    wordToId[occurrence.SpanWords[i]] = id;
                    idToWord.Add(occurrence.SpanWords[i]);
                }
                seq[i] = id;
            }
            sequences.Add(seq);
        }

        // 2) Concatenate with sentinel '0'
        // This produces a flat list containing all snippet-as-int-chain sequences (order doesn't matteR)
        // E.x.: ["the dog barks", "the cat meows"] becomes [1, 2, 3, 0, 1, 4, 5]
        var flattenedWordSequenceIdList = new List<int>();
        foreach (var seq in sequences)
        {
            flattenedWordSequenceIdList.AddRange(seq);
            flattenedWordSequenceIdList.Add(0);
        }

        // 3) Build suffix automaton
        int wordId = nextId;

        // We begin by creating the special "initial state" at index 0. This state is defined
        // by having a length of 0, which means it represents the empty string "". It serves
        // as the root of the automaton. The 'wordId' parameter here is used to set the
        // alphabet size for the state's transition array, not to represent a word.
        var states = new List<AutomatonState> { new AutomatonState(alphabetSize: nextId) };

        // always holds the index of the state corresponding to the entire string processed so far
        int lastStateIndex = 0;

        // Operate on each word id in the flattened list
        for (int i = 0; i < flattenedWordSequenceIdList.Count; i++)
        {
            // 'currentWordId' is the int ID of the word we are currently processing.
            var currentWordId = flattenedWordSequenceIdList[i];

            // =====================================================================
            // == THE "EXTENSION" HAPPENS HERE                                    ==
            // =====================================================================

            // Before additing a new state, get our new state index, which we'll point to later
            int newStateIndex = states.Count;

            // 1. Create a new state to represent the new, longer string.
            AutomatonState newWordState = new AutomatonState(alphabetSize: wordId);
            states.Add(newWordState);

            // 2. Define its length. This is the most explicit part of the extension.
            //    We take the length of the PREVIOUS full string (represented by 'lastStateIndex')
            //    and add one.
            newWordState.Length = states[lastStateIndex].Length + 1;

            // This is the single-line fix.
            // Each primary state created represents one observed occurrence of that prefix.
            newWordState.Count = 1;

            // =====================================================================


            // 'FirstPos' stores the end position of the first occurrence of the substring represented by this state.
            newWordState.FirstOccurrenceEndPosition = i;

            // ... Now the while loop begins, operating on the consequences of the extension

            // Start traversing from the 'last' state up the suffix links.
            // We are looking for a state that already has a transition on the 'currentWordId'.
            // Iteratively, for every traversal node, if it doesn't have a transition for 'currentWordId', add a transition to our new state.
            int traversalNodeIndex = lastStateIndex;
            while (traversalNodeIndex != -1)
            {
                // Get a direct reference to the state we are currently inspecting in the suffix chain.
                var traversalNode = states[traversalNodeIndex];

                // Check if a transition for the current word already exists from this node.
                // If it does, we can stop, because the rest of the logic is handled in the 'if/else' block below.
                if (traversalNode.Next[currentWordId] != -1)
                    break;

                // If no transition exists, create one from this traversalNode to our new state.
                traversalNode.Next[currentWordId] = newStateIndex;

                // Move to the next node in the suffix chain for the next iteration.
                traversalNodeIndex = traversalNode.Link;
            }

            // If traversalNodeIndex is -1, it means we reached the initial state without finding a transition.
            // In this case, the suffix link of our new state points to the initial state (index 0).
            if (traversalNodeIndex == -1)
            {
                newWordState.Link = 0;
            }
            else
            {
                // We found a state ('traversalNodeIndex') that has a transition on 'currentWordId'.
                // Let's call the state it transitions to 'nextStateViaTransition'.
                int nextStateViaTransition = states[traversalNodeIndex].Next[currentWordId];

                // Case 1: The path to 'nextStateViaTransition' corresponds to the string which is exactly one character longer
                // than the string for 'traversalNodeIndex'. This means the transition is "solid".
                if (states[traversalNodeIndex].Length + 1 == states[nextStateViaTransition].Length)
                {
                    // We can simply set the suffix link of our new state to 'nextStateViaTransition'.
                    newWordState.Link = nextStateViaTransition;
                }
                else
                {
                    // Case 2: The path to 'nextStateViaTransition' is longer than necessary. This means we need to split this state.
                    // We create a "cloned" state that will represent the shorter path.
                    int clonedStateIndex = states.Count;
                    states.Add(new AutomatonState(wordId)
                    {
                        Length = states[traversalNodeIndex].Length + 1, // The length is the required shorter length.
                        Next = (int[])states[nextStateViaTransition].Next.Clone(), // It inherits the transitions of the state we're splitting.
                        Link = states[nextStateViaTransition].Link, // It also inherits the suffix link.
                        FirstOccurrenceEndPosition = states[nextStateViaTransition].FirstOccurrenceEndPosition
                    });

                    // Now, we traverse back up from 'traversalNodeIndex', redirecting any transition that pointed to 'nextStateViaTransition'
                    // to our new 'clonedStateIndex' instead.
                    while (traversalNodeIndex != -1 && states[traversalNodeIndex].Next[currentWordId] == nextStateViaTransition)
                    {
                        states[traversalNodeIndex].Next[currentWordId] = clonedStateIndex;
                        traversalNodeIndex = states[traversalNodeIndex].Link;
                    }

                    // The original state we split and our brand new state both get their suffix links pointed to the cloned state.
                    states[nextStateViaTransition].Link = newWordState.Link = clonedStateIndex;
                }
            }
            // Finally, update 'last' to our newly created state for the next iteration.
            lastStateIndex = newStateIndex;
        }

        // 4) Compute end‑pos counts
        var order = Enumerable.Range(0, states.Count).OrderByDescending(i => states[i].Length).ToArray();
        foreach (var i in order)
        {
            if (states[i].Link != -1)
                states[states[i].Link].Count += states[i].Count;
        }

        // 5) Extract maximal repeated spans
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 1; i < states.Count; i++)
        {
            if (states[i].Count <= 1)
                continue;

            bool isMaximal = true;
            for (int j = 1; j < wordId; j++) // Start from 1 to ignore separator
            {
                int nextStateIndex = states[i].Next[j];
                if (nextStateIndex != -1 && states[nextStateIndex].Count == states[i].Count)
                {
                    isMaximal = false;
                    break;
                }
            }

            if (!isMaximal)
                continue;

            int len = states[i].Length;
            int start = states[i].FirstOccurrenceEndPosition - len + 1;

            bool crossesSentinel = false;

            for (int k = start; k < start + len; k++) if (flattenedWordSequenceIdList[k] == 0) { crossesSentinel = true; break; }
            if (crossesSentinel)
                continue;

            var spanWords = flattenedWordSequenceIdList.GetRange(start, len).Select(id => idToWord[id]);
            result[string.Join(' ', spanWords)] = (int)states[i].Count;
        }

        // 6) Add all original full spans to ensure they are included.
        var wholeCounts = allOccurrences
            .GroupBy(s => s.SpanText)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

        foreach (var (text, count) in wholeCounts)
        {
            result.TryAdd(text, count);
        }

        return result;
    }

    /// <summary>
    /// This method is now responsible for enriching the automaton's output with deep context,
    /// including both word-level and token-level adjacencies.
    /// </summary>
    private List<AnalyzedUnmatchedSpan> ConsolidateResults(Dictionary<string, int> allSpans, List<UnmatchedSpanOccurrence> allUnmatchedOccurrences)
    {
        var result = new List<AnalyzedUnmatchedSpan>();
        var originalWholeSpanTexts = allUnmatchedOccurrences.Select(o => o.SpanText).ToHashSet(StringComparer.Ordinal);

        foreach (var (spanText, count) in allSpans.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
        {
            var subSpanContexts = new List<SubSpanContext>();
            // The dictionary key is now a tuple that uniquely identifies an adjacency by its text and type.
            var precedingAdjacencyFreq = new Dictionary<(string Text, Type TokenType), int>();
            var followingAdjacencyFreq = new Dictionary<(string Text, Type TokenType), int>();

            var spanTextWords = spanText.Split(' ');

            foreach (var originalOccurrence in allUnmatchedOccurrences)
            {
                // Find where our current span (e.g., "runs fast") exists inside a larger occurrence (e.g., "The dog runs fast...").
                int wordStartIndex = FindSubArray(originalOccurrence.SpanWords, spanTextWords);
                if (wordStartIndex == -1) continue;

                var spanWordCount = spanTextWords.Length;
                subSpanContexts.Add(new SubSpanContext(originalOccurrence, wordStartIndex, spanWordCount));

                // --- ENHANCED ADJACENCY LOGIC ---

                // 1. Determine what PRECEDES the span.
                if (wordStartIndex == 0)
                {
                    // The span starts at the beginning of the original UnmatchedToken.
                    // Its true predecessor is the Matched Token before the whole thing.
                    var precedingToken = originalOccurrence.PrecedingToken;
                    if (precedingToken != null)
                    {
                        var key = (precedingToken.Value.ToStringValue(), precedingToken.Value.Kind);
                        precedingAdjacencyFreq[key] = precedingAdjacencyFreq.GetValueOrDefault(key) + 1;
                    }
                }
                else
                {
                    // The span starts in the middle of the original UnmatchedToken.
                    // Its predecessor is just the word before it within the same token.
                    string precedingWord = originalOccurrence.SpanWords[wordStartIndex - 1];
                    var key = (precedingWord, (Type)null); // TokenType is null for unmatched words.
                    precedingAdjacencyFreq[key] = precedingAdjacencyFreq.GetValueOrDefault(key) + 1;
                }

                // 2. Determine what FOLLOWS the span.
                if (wordStartIndex + spanWordCount == originalOccurrence.SpanWordCount)
                {
                    // The span ends at the same time as the original UnmatchedToken.
                    // Its true successor is the Matched Token after the whole thing.
                    var followingToken = originalOccurrence.FollowingToken;
                    if (followingToken != null)
                    {
                        var key = (followingToken.Value.ToStringValue(), followingToken.Value.Kind);
                        followingAdjacencyFreq[key] = followingAdjacencyFreq.GetValueOrDefault(key) + 1;
                    }
                }
                else
                {
                    // The span ends in the middle of the original UnmatchedToken.
                    // Its successor is just the word after it within the same token.
                    string followingWord = originalOccurrence.SpanWords[wordStartIndex + spanWordCount];
                    var key = (followingWord, (Type)null); // TokenType is null.
                    followingAdjacencyFreq[key] = followingAdjacencyFreq.GetValueOrDefault(key) + 1;
                }
            }

            // Convert frequency dictionaries to the final rich list format.
            var precedingAdjacencies = precedingAdjacencyFreq
                .Select(kv => new SpanAdjacency(kv.Key.Text, kv.Key.TokenType, kv.Value))
                .ToList();

            var followingAdjacencies = followingAdjacencyFreq
                .Select(kv => new SpanAdjacency(kv.Key.Text, kv.Key.TokenType, kv.Value))
                .ToList();

            result.Add(new AnalyzedUnmatchedSpan(
                text: spanText,
                frequency: count,
                isFullSpan: originalWholeSpanTexts.Contains(spanText),
                occurrences: subSpanContexts,
                precedingAdjacencies: precedingAdjacencies,
                followingAdjacencies: followingAdjacencies
            ));
        }

        return result;
    }

    // Helper to find a subarray (sequence of words) within another.
    private static int FindSubArray(string[] array, string[] subarray)
    {
        for (int i = 0; i <= array.Length - subarray.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < subarray.Length; j++)
            {
                if (array[i + j] != subarray[j])
                {
                    found = false;
                    break;
                }
            }
            if (found) return i;
        }
        return -1;
    }
    #endregion
}