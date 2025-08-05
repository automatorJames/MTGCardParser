namespace MTGPlexer.TokenAnalysis;

public record UnmatchedDigestedCorpus
{
    public List<AnalyzedUnmatchedSpan> Spans { get; }

    public UnmatchedDigestedCorpus(List<UnmatchedSpanOccurrence> allOccurrences)
    {
        Spans = RunDigestionAutomaton(allOccurrences);
    }

    private List<AnalyzedUnmatchedSpan> RunDigestionAutomaton(List<UnmatchedSpanOccurrence> allOccurrences)
    {
        // =================================================================================
        // == STEPS 1 & 2: Suffix Automaton construction (Unchanged)                      ==
        // =================================================================================
        // ... (The entire automaton building logic is identical to the previous version)
        var wordToId = new Dictionary<string, int>(StringComparer.Ordinal);
        var idToWord = new List<string>();
        var flattenedWordSequenceIdList = new List<int>();
        var indexToOccurrenceMap = new List<UnmatchedSpanOccurrence>();
        int nextWordId = 0;
        int nextCorrelationId = -1;

        foreach (var occurrence in allOccurrences)
        {
            foreach (var word in occurrence.Words)
            {
                if (!wordToId.TryGetValue(word, out var id)) { id = nextWordId++; wordToId[word] = id; idToWord.Add(word); }
                flattenedWordSequenceIdList.Add(id);
                indexToOccurrenceMap.Add(occurrence);
            }
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
            states.Add(new AutomatonState(alphabetSize) { Length = states[lastStateIndex].Length + 1, Count = 1, FirstOccurrenceEndPosition = i });
            int p = lastStateIndex;
            while (p != -1 && states[p].Next[currentId] == -1) { states[p].Next[currentId] = newStateIndex; p = states[p].Link; }
            if (p == -1) { states[newStateIndex].Link = 0; }
            else
            {
                int q = states[p].Next[currentId];
                if (states[q].Length == states[p].Length + 1) { states[newStateIndex].Link = q; }
                else
                {
                    int cloneIndex = states.Count;
                    var qState = states[q];
                    states.Add(new AutomatonState(alphabetSize) { Length = states[p].Length + 1, Next = (int[])qState.Next.Clone(), Link = qState.Link, FirstOccurrenceEndPosition = qState.FirstOccurrenceEndPosition });
                    while (p != -1 && states[p].Next[currentId] == q) { states[p].Next[currentId] = cloneIndex; p = states[p].Link; }
                    qState.Link = states[newStateIndex].Link = cloneIndex;
                }
            }
            lastStateIndex = newStateIndex;
        }
        var order = Enumerable.Range(0, states.Count).OrderByDescending(i => states[i].Length).ToArray();
        foreach (var i in order) { if (states[i].Link != -1) states[states[i].Link].Count += states[i].Count; }

        // =================================================================================
        // == STEP 3: Extract Spans and Consolidate with Rich, Key-Based Contexts         ==
        // =================================================================================
        var result = new List<AnalyzedUnmatchedSpan>();
        var originalWholeSpanTexts = allOccurrences.Select(o => o.Text).ToHashSet(StringComparer.Ordinal);
        var allMaximalSpans = new Dictionary<string, int>(StringComparer.Ordinal);

        for (int i = 1; i < states.Count; i++)
        {
            if (states[i].Count <= 1) continue;
            bool isMaximal = true;
            for (int j = 0; j < alphabetSize; j++) { if (states[i].Next[j] != -1 && states[states[i].Next[j]].Count == states[i].Count) { isMaximal = false; break; } }
            if (!isMaximal) continue;
            int len = states[i].Length;
            int start = states[i].FirstOccurrenceEndPosition - len + 1;
            var spanText = string.Join(' ', flattenedWordSequenceIdList.GetRange(start, len).Select(id => idToWord[id]));
            if (!allMaximalSpans.ContainsKey(spanText) || allMaximalSpans[spanText] < states[i].Count) { allMaximalSpans[spanText] = (int)states[i].Count; }
        }

        var wholeCounts = allOccurrences.GroupBy(s => s.Text).ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);
        foreach (var (text, count) in wholeCounts) { if (!allMaximalSpans.ContainsKey(text)) { allMaximalSpans.Add(text, count); } }

        foreach (var (spanText, count) in allMaximalSpans.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
        {
            var subSpanContexts = new List<UnmatchedSubSpanContext>();
            var precedingSequencesWithKeys = new List<(List<(string Text, Type TokenType)> Sequence, CardSpanKey Key)>();
            var followingSequencesWithKeys = new List<(List<(string Text, Type TokenType)> Sequence, CardSpanKey Key)>();
            var spanTextWords = spanText.Split(' ');
            var allOccurrenceIndices = FindAllOccurrences(flattenedWordSequenceIdList, spanTextWords.Select(w => wordToId[w]).ToArray());

            foreach (int startIndexInFlatList in allOccurrenceIndices)
            {
                var originalOccurrence = indexToOccurrenceMap[startIndexInFlatList];
                int wordStartIndexInSource = -1;
                for (int i = 0; i <= originalOccurrence.Words.Length - spanTextWords.Length; i++)
                {
                    if (originalOccurrence.Words.Skip(i).Take(spanTextWords.Length).SequenceEqual(spanTextWords, StringComparer.Ordinal)) { wordStartIndexInSource = i; break; }
                }
                if (wordStartIndexInSource == -1) continue;
                subSpanContexts.Add(new UnmatchedSubSpanContext(originalOccurrence, wordStartIndexInSource, spanTextWords.Length));

                var precedingSequence = new List<(string Text, Type TokenType)>();
                var followingSequence = new List<(string Text, Type TokenType)>();

                if (originalOccurrence.PrecedingToken is { } pt) { precedingSequence.Add((pt.ToStringValue(), pt.Kind)); }
                for (int i = 0; i < wordStartIndexInSource; i++) { precedingSequence.Add((originalOccurrence.Words[i], null)); }
                if (precedingSequence.Any()) { precedingSequencesWithKeys.Add((precedingSequence, originalOccurrence.Key)); }

                int followingWordIndex = wordStartIndexInSource + spanTextWords.Length;
                for (int i = followingWordIndex; i < originalOccurrence.Words.Length; i++) { followingSequence.Add((originalOccurrence.Words[i], null)); }
                if (originalOccurrence.FollowingToken is { } ft) { followingSequence.Add((ft.ToStringValue(), ft.Kind)); }
                if (followingSequence.Any()) { followingSequencesWithKeys.Add((followingSequence, originalOccurrence.Key)); }
            }

            var precedingAdjacencyTree = BuildAdjacencyTree(precedingSequencesWithKeys.Select(s => { s.Sequence.Reverse(); return s; }).ToList());
            var followingAdjacencyTree = BuildAdjacencyTree(followingSequencesWithKeys);

            // Reintroduce the consolidation step
            var collapsedPrecedingTree = CollapseAdjacencyNodes(precedingAdjacencyTree, isReversed: true);
            var collapsedFollowingTree = CollapseAdjacencyNodes(followingAdjacencyTree, isReversed: false);

            result.Add(new AnalyzedUnmatchedSpan(
                text: spanText,
                maximalSpanOccurrenceCount: count,
                occurrences: subSpanContexts,
                precedingAdjacencies: collapsedPrecedingTree,
                followingAdjacencies: collapsedFollowingTree
            ));
        }
        return result;
    }

    private List<AdjacencyNode> BuildAdjacencyTree(List<(List<(string Text, Type TokenType)> Sequence, CardSpanKey Key)> sequencesWithKeys)
    {
        if (sequencesWithKeys == null || !sequencesWithKeys.Any(x => x.Sequence.Any())) return new List<AdjacencyNode>();
        var nodeGroups = sequencesWithKeys.Where(x => x.Sequence.Any()).GroupBy(x => x.Sequence.First());
        var nodes = new List<AdjacencyNode>();
        foreach (var group in nodeGroups)
        {
            var (text, tokenType) = group.Key;
            var sourceOccurrences = group.Select(g => g.Key).Distinct().ToList(); // Use Distinct for safety
            var childSequencesWithKeys = group.Select(x => (Sequence: x.Sequence.Skip(1).ToList(), x.Key)).Where(x => x.Sequence.Any()).ToList();
            var children = BuildAdjacencyTree(childSequencesWithKeys);

            // Create a node with a single segment, ready for potential consolidation
            nodes.Add(new AdjacencyNode(
                segments: new List<NodeSegment> { new(text, tokenType) },
                sourceOccurrences: sourceOccurrences,
                children: children
            ));
        }
        return nodes;
    }

    /// <summary>
    /// Consolidates linear paths in the adjacency tree into single nodes with multiple segments.
    /// </summary>
    private List<AdjacencyNode> CollapseAdjacencyNodes(List<AdjacencyNode> nodes, bool isReversed)
    {
        if (nodes == null || !nodes.Any()) return new List<AdjacencyNode>();

        var collapsedNodes = new List<AdjacencyNode>();
        foreach (var node in nodes)
        {
            var currentNode = node;
            var allSegments = new List<NodeSegment>(currentNode.Segments);

            // Condition for collapsing: one child that shares the exact same set of source occurrences.
            while (currentNode.Children.Count == 1 &&
                   currentNode.Children[0].SourceOccurrences.Count == currentNode.SourceOccurrences.Count &&
                   currentNode.Children[0].SourceOccurrences.All(currentNode.SourceOccurrences.Contains))
            {
                currentNode = currentNode.Children[0];
                allSegments.AddRange(currentNode.Segments); // Add the child's segments
            }

            // Recurse on the children of the *last* node in the chain
            var newChildren = CollapseAdjacencyNodes(currentNode.Children, isReversed);

            if (isReversed)
            {
                allSegments.Reverse();
            }

            collapsedNodes.Add(new AdjacencyNode(
                segments: allSegments,
                sourceOccurrences: node.SourceOccurrences, // Use the original node's occurrences
                children: newChildren
            ));
        }
        return collapsedNodes;
    }

    // FindAllOccurrences and AutomatonState are unchanged...
    private List<int> FindAllOccurrences(List<int> source, int[] pattern)
    {
        var indices = new List<int>();
        if (pattern.Length == 0 || source.Count < pattern.Length) return indices;
        for (int i = 0; i <= source.Count - pattern.Length; i++)
        {
            if (source[i] < 0) continue;
            bool match = true;
            for (int j = 0; j < pattern.Length; j++) { if (source[i + j] != pattern[j]) { match = false; break; } }
            if (match) indices.Add(i);
        }
        return indices;
    }
    private class AutomatonState
    {
        public int[] Next; public int Link; public int Length; public long Count; public int FirstOccurrenceEndPosition;
        public AutomatonState(int alphabetSize) { Next = new int[alphabetSize]; Array.Fill(Next, -1); Link = -1; }
    }
}