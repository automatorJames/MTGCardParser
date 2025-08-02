namespace MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

/// <summary>
/// A record that encapsulates the entire corpus digestion and analysis process using a suffix automaton
/// with precise context tracking. It correctly identifies maximal repeated spans and attributes their
/// surrounding context (adjacencies) by tracking the origin of every word.
/// </summary>
public record UnmatchedDigestedCorpus
{
    /// <summary>
    /// The final, consolidated list of all unique unmatched spans (maximal sub-spans)
    /// found in the corpus, with correctly attributed contextual information.
    /// </summary>
    public List<AnalyzedUnmatchedSpan> Spans { get; }

    public UnmatchedDigestedCorpus(List<UnmatchedSpanOccurrence> allOccurrences)
    {
        Spans = RunDigestionAutomaton(allOccurrences);
    }

    /// <summary>
    /// The core automaton logic, adapted to track and consolidate contexts accurately.
    /// </summary>
    private List<AnalyzedUnmatchedSpan> RunDigestionAutomaton(List<UnmatchedSpanOccurrence> allOccurrences)
    {
        // =================================================================================
        // == STEP 1: Build Word Mappings and the Flattened Sequence                      ==
        // =================================================================================

        var wordToId = new Dictionary<string, int>(StringComparer.Ordinal);
        var idToWord = new List<string>();
        var flattenedWordSequenceIdList = new List<int>();

        // This is the crucial new data structure for accurate context tracking.
        // It maps every index in `flattenedWordSequenceIdList` back to the original occurrence it came from.
        var indexToOccurrenceMap = new List<UnmatchedSpanOccurrence>();

        int nextWordId = 0; // 0 is now a valid word id. We use negative numbers for sentinels.
        int nextCorrelationId = -1;

        foreach (var occurrence in allOccurrences)
        {
            // Add the words from the current occurrence
            foreach (var word in occurrence.Words)
            {
                if (!wordToId.TryGetValue(word, out var id))
                {
                    id = nextWordId++;
                    wordToId[word] = id;
                    idToWord.Add(word);
                }
                flattenedWordSequenceIdList.Add(id);
                indexToOccurrenceMap.Add(occurrence); // Map this word's index to its source
            }

            // Add a unique, negative sentinel to signify the end of this specific occurrence
            flattenedWordSequenceIdList.Add(nextCorrelationId);
            indexToOccurrenceMap.Add(occurrence); // The sentinel also belongs to the source

            nextCorrelationId--;
        }

        // =================================================================================
        // == STEP 2: Build the Suffix Automaton                                          ==
        // =================================================================================

        int alphabetSize = nextWordId;
        var states = new List<AutomatonState> { new AutomatonState(alphabetSize) };
        int lastStateIndex = 0;

        for (int i = 0; i < flattenedWordSequenceIdList.Count; i++)
        {
            var currentId = flattenedWordSequenceIdList[i];

            // We only build the automaton on actual words (non-negative IDs)
            if (currentId < 0)
            {
                // When we hit a sentinel, we reset the `lastStateIndex` to 0. This ensures that
                // the next word starts a new path from the root, preventing the creation of
                // spans that cross different original occurrences.
                lastStateIndex = 0;
                continue;
            }

            int newStateIndex = states.Count;
            var newWordState = new AutomatonState(alphabetSize)
            {
                Length = states[lastStateIndex].Length + 1,
                Count = 1,
                FirstOccurrenceEndPosition = i
            };
            states.Add(newWordState);

            int traversalNodeIndex = lastStateIndex;
            while (traversalNodeIndex != -1 && states[traversalNodeIndex].Next[currentId] == -1)
            {
                states[traversalNodeIndex].Next[currentId] = newStateIndex;
                traversalNodeIndex = states[traversalNodeIndex].Link;
            }

            if (traversalNodeIndex == -1)
            {
                newWordState.Link = 0;
            }
            else
            {
                int nextStateViaTransition = states[traversalNodeIndex].Next[currentId];
                if (states[traversalNodeIndex].Length + 1 == states[nextStateViaTransition].Length)
                {
                    newWordState.Link = nextStateViaTransition;
                }
                else
                {
                    int clonedStateIndex = states.Count;
                    var existingState = states[nextStateViaTransition];
                    states.Add(new AutomatonState(alphabetSize)
                    {
                        Length = states[traversalNodeIndex].Length + 1,
                        Next = (int[])existingState.Next.Clone(),
                        Link = existingState.Link,
                        FirstOccurrenceEndPosition = existingState.FirstOccurrenceEndPosition
                    });

                    while (traversalNodeIndex != -1 && states[traversalNodeIndex].Next[currentId] == nextStateViaTransition)
                    {
                        states[traversalNodeIndex].Next[currentId] = clonedStateIndex;
                        traversalNodeIndex = states[traversalNodeIndex].Link;
                    }
                    existingState.Link = newWordState.Link = clonedStateIndex;
                }
            }
            lastStateIndex = newStateIndex;
        }

        var order = Enumerable.Range(0, states.Count).OrderByDescending(i => states[i].Length).ToArray();
        foreach (var i in order)
        {
            if (states[i].Link != -1)
                states[states[i].Link].Count += states[i].Count;
        }

        // =================================================================================
        // == STEP 3: Extract Spans and Consolidate with Accurate Contexts                ==
        // =================================================================================

        var result = new List<AnalyzedUnmatchedSpan>();
        var originalWholeSpanTexts = allOccurrences.Select(o => o.Text).ToHashSet(StringComparer.Ordinal);
        var allMaximalSpans = new Dictionary<string, (int count, int stateIndex)>(StringComparer.Ordinal);

        // First, find all maximal spans and their counts from the automaton states
        for (int i = 1; i < states.Count; i++)
        {
            if (states[i].Count <= 1) continue;

            bool isMaximal = true;
            for (int j = 0; j < alphabetSize; j++)
            {
                int nextStateIndex = states[i].Next[j];
                if (nextStateIndex != -1 && states[nextStateIndex].Count == states[i].Count)
                {
                    isMaximal = false;
                    break;
                }
            }
            if (!isMaximal) continue;

            int len = states[i].Length;
            int start = states[i].FirstOccurrenceEndPosition - len + 1;

            // This is the line that caused the error. The logic has been adjusted during automaton
            // creation (resetting lastStateIndex) to prevent invalid spans from being formed in the first place,
            // making this check unnecessary and the line below safe.
            var spanWords = flattenedWordSequenceIdList.GetRange(start, len).Select(id => idToWord[id]);
            var spanText = string.Join(' ', spanWords);

            // In cases of multiple states representing the same text, we keep the one with the higher count.
            if (!allMaximalSpans.ContainsKey(spanText) || allMaximalSpans[spanText].count < states[i].Count)
            {
                allMaximalSpans[spanText] = ((int)states[i].Count, i);
            }
        }

        // Ensure all original full spans are included, even if not maximal repeats
        var wholeCounts = allOccurrences
            .GroupBy(s => s.Text)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

        foreach (var (text, count) in wholeCounts)
        {
            if (!allMaximalSpans.ContainsKey(text))
            {
                // We don't have a state index, so use -1 as a placeholder
                allMaximalSpans.Add(text, (count, -1));
            }
        }

        // Now, process each unique span we've found
        foreach (var (spanText, (count, stateIndex)) in allMaximalSpans.OrderByDescending(kv => kv.Value.count).ThenBy(kv => kv.Key))
        {
            var subSpanContexts = new List<UnmatchedSubSpanContext>();
            var followingSequences = new List<List<(string Text, Type TokenType)>>();
            var precedingSequences = new List<List<(string Text, Type TokenType)>>();

            var spanTextWords = spanText.Split(' ');
            var spanWordIds = spanTextWords.Select(w => wordToId[w]).ToArray();

            // Find all actual occurrences of this span in the flattened list
            var allOccurrenceIndices = FindAllOccurrences(flattenedWordSequenceIdList, spanWordIds);

            foreach (int startIndexInFlatList in allOccurrenceIndices)
            {
                // THIS IS THE KEY: Use the map to get the TRUE original context
                var originalOccurrence = indexToOccurrenceMap[startIndexInFlatList];

                // We need to find the start index of the span *relative to its own occurrence*
                int wordStartIndexInSource = -1;
                for (int i = 0; i <= originalOccurrence.Words.Length - spanTextWords.Length; i++)
                {
                    if (originalOccurrence.Words.Skip(i).Take(spanTextWords.Length).SequenceEqual(spanTextWords, StringComparer.Ordinal))
                    {
                        wordStartIndexInSource = i;
                        break;
                    }
                }

                if (wordStartIndexInSource == -1) continue; // Should not happen with this logic, but as a safeguard.

                subSpanContexts.Add(new UnmatchedSubSpanContext(originalOccurrence, wordStartIndexInSource, spanTextWords.Length));

                // Extract the PRECEDING sequence from the true source
                var precedingSequence = new List<(string Text, Type TokenType)>();
                if (originalOccurrence.PrecedingToken is { } pt)
                    precedingSequence.Add((pt.ToStringValue(), pt.Kind));
                for (int i = 0; i < wordStartIndexInSource; i++)
                    precedingSequence.Add((originalOccurrence.Words[i], null));
                if (precedingSequence.Any())
                    precedingSequences.Add(precedingSequence);

                // Extract the FOLLOWING sequence from the true source
                var followingSequence = new List<(string Text, Type TokenType)>();
                for (int i = wordStartIndexInSource + spanTextWords.Length; i < originalOccurrence.SpanWordCount; i++)
                    followingSequence.Add((originalOccurrence.Words[i], null));
                if (originalOccurrence.FollowingToken is { } ft)
                    followingSequence.Add((ft.ToStringValue(), ft.Kind));
                if (followingSequence.Any())
                    followingSequences.Add(followingSequence);
            }

            // The rest of the logic (building and collapsing trees) is the same as before
            var precedingAdjacencyTree = BuildAdjacencyTree(precedingSequences.Select(s => { s.Reverse(); return s; }).ToList());
            var followingAdjacencyTree = BuildAdjacencyTree(followingSequences);

            var collapsedPrecedingTree = CollapseAdjacencyNodes(precedingAdjacencyTree, isReversed: true);
            var collapsedFollowingTree = CollapseAdjacencyNodes(followingAdjacencyTree, isReversed: false);
            CalculateTreeLayout(collapsedPrecedingTree);
            CalculateTreeLayout(collapsedFollowingTree);

            result.Add(new AnalyzedUnmatchedSpan(
                text: spanText,
                maximalSpanOccurrenceCount: count,
                isFullSpan: originalWholeSpanTexts.Contains(spanText),
                occurrences: subSpanContexts,
                precedingAdjacencies: collapsedPrecedingTree,
                followingAdjacencies: collapsedFollowingTree
            ));
        }

        return result;
    }

    /// <summary>
    /// Recursively traverses the adjacency tree to calculate the vertical lane position
    /// and total lane span for each node, enabling a stable and predictable UI layout.
    /// </summary>
    /// <returns>The total number of vertical lanes required by this level of the tree.</returns>
    private int CalculateTreeLayout(List<AdjacencyNode> nodes, int startLane = 0)
    {
        int currentLane = startLane;
        foreach (var node in nodes)
        {
            node.VerticalLane = currentLane;

            if (node.Children.Any())
            {
                node.TotalDescendantLanes = CalculateTreeLayout(node.Children, currentLane);
                currentLane = node.TotalDescendantLanes;
            }
            else
            {
                node.TotalDescendantLanes = currentLane + 1;
                currentLane++;
            }
        }
        return currentLane;
    }

    private List<int> FindAllOccurrences(List<int> source, int[] pattern)
    {
        var indices = new List<int>();
        if (pattern.Length == 0 || source.Count < pattern.Length) return indices;

        for (int i = 0; i <= source.Count - pattern.Length; i++)
        {
            // Skip checks that would start inside a sentinel
            if (source[i] < 0) continue;

            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (source[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                indices.Add(i);
            }
        }
        return indices;
    }

    private List<AdjacencyNode> BuildAdjacencyTree(List<List<(string Text, Type TokenType)>> sequences)
    {
        if (sequences == null || !sequences.Any(x => x.Any()))
            return new List<AdjacencyNode>();

        var nodeGroups = sequences
            .Where(x => x.Any())
            .GroupBy(x => x.First());

        var nodes = new List<AdjacencyNode>();
        foreach (var group in nodeGroups)
        {
            var (text, tokenType) = group.Key;
            var frequency = group.Count();

            var childSequences = group
                .Select(x => x.Skip(1).ToList())
                .Where(x => x.Any())
                .ToList();

            var children = BuildAdjacencyTree(childSequences);

            nodes.Add(new AdjacencyNode(text, tokenType, frequency, children));
        }
        return nodes;
    }

    private List<AdjacencyNode> CollapseAdjacencyNodes(List<AdjacencyNode> nodes, bool isReversed)
    {
        if (nodes == null || !nodes.Any()) return new List<AdjacencyNode>();

        var collapsedNodes = new List<AdjacencyNode>();
        foreach (var node in nodes)
        {
            var currentNode = node;
            var textParts = new List<string> { currentNode.Text };

            while (currentNode.Children.Count == 1 &&
                   currentNode.Children[0].Frequency == currentNode.Frequency &&
                   currentNode.Children[0].TokenType == null)
            {
                currentNode = currentNode.Children[0];
                textParts.Add(currentNode.Text);
            }

            var newChildren = CollapseAdjacencyNodes(currentNode.Children, isReversed);

            if (isReversed)
            {
                textParts.Reverse();
            }

            collapsedNodes.Add(new AdjacencyNode(
                text: string.Join(" ", textParts),
                tokenType: node.TokenType,
                frequency: node.Frequency,
                children: newChildren
            ));
        }
        return collapsedNodes;
    }

    /// <summary>
    /// Represents a single state (or node) in a suffix automaton.
    /// </summary>
    private class AutomatonState
    {
        public int[] Next;
        public int Link;
        public int Length;
        public long Count;
        public int FirstOccurrenceEndPosition;

        public AutomatonState(int alphabetSize)
        {
            Next = new int[alphabetSize];
            Array.Fill(Next, -1);
            Link = -1;
        }
    }
}