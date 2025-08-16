using System.Text;

namespace MTGPlexer.TokenAnalysis.SpanDTOs;

public record DigestedSpanCorpus
{
    public List<AnalyzedSpan> Spans { get; }

    public DigestedSpanCorpus(List<SpanOccurrence> allOccurrences)
    {
        Spans = RunDigestionAutomaton(allOccurrences);
    }

    private List<AnalyzedSpan> RunDigestionAutomaton(List<SpanOccurrence> allOccurrences)
    {
        // =================================================================================
        // == STEPS 1 & 2: Suffix Automaton construction (Unchanged)                      ==
        // =================================================================================
        var wordToId = new Dictionary<string, int>(StringComparer.Ordinal);
        var idToWord = new List<string>();
        var flattenedWordSequenceIdList = new List<int>();
        var indexToOccurrenceMap = new List<SpanOccurrence>();
        int nextWordId = 0;
        int nextCorrelationId = -1;

        foreach (var occurrence in allOccurrences)
        {
            foreach (var word in occurrence.Words)
            {
                if (!wordToId.TryGetValue(word, out var id))
                {
                    id = nextWordId++;
                    wordToId[word] = id;
                    idToWord.Add(word);
                }
                flattenedWordSequenceIdList.Add(id);
                indexToOccurrenceMap.Add(occurrence);
            }
            // correlation boundary
            flattenedWordSequenceIdList.Add(nextCorrelationId);
            indexToOccurrenceMap.Add(occurrence);
            nextCorrelationId--;
        }

        int alphabetSize = nextWordId;
        var states = new List<AutomatonState> { new(alphabetSize) };
        int lastStateIndex = 0;

        for (int i = 0; i < flattenedWordSequenceIdList.Count; i++)
        {
            var currentId = flattenedWordSequenceIdList[i];
            if (currentId < 0) { lastStateIndex = 0; continue; }

            int newStateIndex = states.Count;
            states.Add(new AutomatonState(alphabetSize)
            {
                Length = states[lastStateIndex].Length + 1,
                Count = 1,
                FirstOccurrenceEndPosition = i
            });

            int p = lastStateIndex;
            while (p != -1 && states[p].Next[currentId] == -1)
            {
                states[p].Next[currentId] = newStateIndex;
                p = states[p].Link;
            }

            if (p == -1)
            {
                states[newStateIndex].Link = 0;
            }
            else
            {
                int q = states[p].Next[currentId];
                if (states[q].Length == states[p].Length + 1)
                {
                    states[newStateIndex].Link = q;
                }
                else
                {
                    int cloneIndex = states.Count;
                    var qState = states[q];
                    states.Add(new AutomatonState(alphabetSize)
                    {
                        Length = states[p].Length + 1,
                        Next = (int[])qState.Next.Clone(),
                        Link = qState.Link,
                        FirstOccurrenceEndPosition = qState.FirstOccurrenceEndPosition
                    });
                    while (p != -1 && states[p].Next[currentId] == q)
                    {
                        states[p].Next[currentId] = cloneIndex;
                        p = states[p].Link;
                    }
                    qState.Link = states[newStateIndex].Link = cloneIndex;
                }
            }
            lastStateIndex = newStateIndex;
        }

        var order = Enumerable.Range(0, states.Count).OrderByDescending(i => states[i].Length).ToArray();
        foreach (var i in order)
        {
            if (states[i].Link != -1) states[states[i].Link].Count += states[i].Count;
        }

        // =================================================================================
        // == STEP 3: Extract Spans and Consolidate with Rich, Key-Based Contexts         ==
        // =================================================================================
        var result = new List<AnalyzedSpan>();
        var allMaximalSpans = new Dictionary<string, int>(StringComparer.Ordinal);

        for (int i = 1; i < states.Count; i++)
        {
            if (states[i].Count <= 1) continue;

            bool isMaximal = true;
            for (int j = 0; j < alphabetSize; j++)
            {
                if (states[i].Next[j] != -1 && states[states[i].Next[j]].Count == states[i].Count)
                {
                    isMaximal = false; break;
                }
            }
            if (!isMaximal) continue;

            int len = states[i].Length;
            int start = states[i].FirstOccurrenceEndPosition - len + 1;
            var spanText = string.Join(' ', flattenedWordSequenceIdList
                .GetRange(start, len)
                .Select(id => idToWord[id]));

            if (!allMaximalSpans.ContainsKey(spanText) || allMaximalSpans[spanText] < states[i].Count)
            {
                allMaximalSpans[spanText] = (int)states[i].Count;
            }
        }

        // include whole-span counts too
        var wholeCounts = allOccurrences
            .GroupBy(s => s.Text)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

        foreach (var (text, count) in wholeCounts)
        {
            if (!allMaximalSpans.ContainsKey(text))
                allMaximalSpans.Add(text, count);
        }

        foreach (var (spanText, count) in allMaximalSpans.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
        {
            var subSpanContexts = new List<SubSpanContext>();
            var precedingSequencesWithKeys = new List<(List<TokenInfo> Sequence, CardSpanKey Key)>();
            var followingSequencesWithKeys = new List<(List<TokenInfo> Sequence, CardSpanKey Key)>();

            var spanTextWords = spanText.Split(' ');
            var allOccurrenceIndices = FindAllOccurrences(
                flattenedWordSequenceIdList,
                spanTextWords.Select(w => wordToId[w]).ToArray());

            foreach (int startIndexInFlatList in allOccurrenceIndices)
            {
                var originalSpanOccurrence = indexToOccurrenceMap[startIndexInFlatList];
                int wordStartIndexInSource = -1;

                for (int i = 0; i <= originalSpanOccurrence.Words.Length - spanTextWords.Length; i++)
                {
                    if (originalSpanOccurrence.Words.Skip(i).Take(spanTextWords.Length).SequenceEqual(spanTextWords, StringComparer.Ordinal))
                    {
                        wordStartIndexInSource = i; break;
                    }
                }

                if (wordStartIndexInSource == -1) continue;

                subSpanContexts.Add(new SubSpanContext(originalSpanOccurrence, wordStartIndexInSource, spanTextWords.Length));

                var precedingSequence = new List<TokenInfo>();
                var followingSequence = new List<TokenInfo>();

                // Process PRECEDING sequences: nearest → farthest
                for (int i = wordStartIndexInSource - 1; i >= 0; i--)
                    precedingSequence.Add(new(originalSpanOccurrence.Words[i], null));

                for (int i = originalSpanOccurrence.AnchorTokenIndex - 1; i >= 0; i--)
                {
                    var token = originalSpanOccurrence.LineTokens[i];
                    Type type = token.Kind == typeof(DefaultUnmatchedString) ? null : token.Kind;
                    precedingSequence.Add(new(token.ToStringValue(), type));
                }

                if (precedingSequence.Any())
                    precedingSequencesWithKeys.Add((precedingSequence, originalSpanOccurrence.Key));
                
                // Build FOLLOWING sequences: nearest → farthest
                int followingWordIndex = wordStartIndexInSource + spanTextWords.Length;
                for (int i = followingWordIndex; i < originalSpanOccurrence.Words.Length; i++)
                    followingSequence.Add(new(originalSpanOccurrence.Words[i], null));

                // Process FOLLOWING tokens nearest → farthest
                for (int i = originalSpanOccurrence.AnchorTokenIndex + 1; i < originalSpanOccurrence.LineTokens.Length; i++)
                {
                    var token = originalSpanOccurrence.LineTokens[i];
                    Type type = token.Kind == typeof(DefaultUnmatchedString) ? null : token.Kind;
                    followingSequence.Add(new(token.ToStringValue(), type));
                }

                if (followingSequence.Any())
                    followingSequencesWithKeys.Add((followingSequence, originalSpanOccurrence.Key));
            }

            //Reverse collapsed text for PRECEDING so phrases read left→right (farthest→nearest)
            var precedingAdjacencyTree = BuildAdjacencyTree(precedingSequencesWithKeys, reverseCollapsedText: true);
            var followingAdjacencyTree = BuildAdjacencyTree(followingSequencesWithKeys, reverseCollapsedText: false);

            result.Add(new AnalyzedSpan(
                text: spanText,
                maximalSpanOccurrenceCount: count,
                occurrences: subSpanContexts,
                precedingAdjacencies: precedingAdjacencyTree,
                followingAdjacencies: followingAdjacencyTree
            ));
        }
        return result;
    }

    /// <summary>
    /// Builds an adjacency tree from token sequences. It identifies any non-branching
    /// path and collapses it into a single AdjacencyNode.
    /// </summary>
    private List<AdjacencyNode> BuildAdjacencyTree(
        List<(List<TokenInfo> Sequence, CardSpanKey Key)> sequencesWithKeys,
        bool reverseCollapsedText)
    {
        if (sequencesWithKeys == null || !sequencesWithKeys.Any(x => x.Sequence.Any()))
            return [];

        var nodeGroups = sequencesWithKeys.Where(x => x.Sequence.Any()).GroupBy(x => x.Sequence.First());
        var nodes = new List<AdjacencyNode>();

        foreach (var group in nodeGroups)
        {
            var initialToken = group.Key;
            var sourceOccurrences = group.Select(g => g.Key).Distinct().ToList();

            // Collect the collapsed linear segment
            var segmentsToCollapse = new List<(string Text, DeterministicPalette Palette)>
            {
                (initialToken.Text, initialToken.TokenType is null ? null : TokenTypeRegistry.Palettes.GetValueOrDefault(initialToken.TokenType))
            };

            var remainingSequences = group.Select(x => (Sequence: x.Sequence.Skip(1).ToList(), x.Key)).ToList();

            // Keep collapsing while the path is linear and shared
            while (true)
            {
                var continuations = remainingSequences.Where(x => x.Sequence.Any()).ToList();

                if (!continuations.Any() || continuations.Count != group.Count())
                    break;

                var nextToken = continuations.First().Sequence.First();

                if (!continuations.All(c => c.Sequence.First().Equals(nextToken)))
                    break;

                segmentsToCollapse.Add((nextToken.Text, nextToken.TokenType is null ? null : TokenTypeRegistry.Palettes.GetValueOrDefault(nextToken.TokenType)));

                // consume one token
                remainingSequences = remainingSequences.Select(x => (Sequence: x.Sequence.Skip(1).ToList(), x.Key)).ToList();
            }

            // Recurse on what remains after collapsing
            var children = BuildAdjacencyTree(remainingSequences, reverseCollapsedText);

            // Build the final combined text and palette map.
            // For PRECEDING trees we reverse the collapsed segment order so the phrase reads farthest→nearest.
            var finalTextBuilder = new StringBuilder();
            var palettes = new Dictionary<int, DeterministicPalette>();

            var segmentIter = reverseCollapsedText
                ? segmentsToCollapse.AsEnumerable().Reverse()
                : segmentsToCollapse;

            foreach (var (segmentText, segmentPalette) in segmentIter)
            {
                if (finalTextBuilder.Length > 0)
                    finalTextBuilder.Append(' ');

                int startIndex = finalTextBuilder.Length;
                palettes[startIndex] = segmentPalette;
                finalTextBuilder.Append(segmentText);
            }

            var finalSegment = new NodeSegment(
                Text: finalTextBuilder.ToString(),
                Palettes: palettes.Count > 0 ? palettes : null
            );

            nodes.Add(new AdjacencyNode(
                segment: finalSegment,
                sourceOccurrences: sourceOccurrences,
                children: children
            ));
        }
        return nodes;
    }

    private List<int> FindAllOccurrences(List<int> source, int[] pattern)
    {
        var indices = new List<int>();
        if (pattern.Length == 0 || source.Count < pattern.Length) return indices;
        for (int i = 0; i <= source.Count - pattern.Length; i++)
        {
            if (source[i] < 0) continue; // boundary
            bool match = true;

            for (int j = 0; j < pattern.Length; j++)
                if (source[i + j] != pattern[j]) { match = false; break; }

            if (match) indices.Add(i);
        }
        return indices;
    }

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