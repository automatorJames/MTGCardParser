namespace Glyphotype.GlyphAnalysisDTOs.WordTrees;

public class DigestedText
{
    public List<AnalyzedText> Spans { get; }

    public DigestedText(List<ProcessedDocument> processedDocuments)
    {
        var allUnmatchedOccurrences = processedDocuments
            .SelectMany(document => document.Lines)
            .SelectMany(line => line.UnmatchedTextOccurrences)
            .ToList();

        Spans = RunDigestionAutomaton(allUnmatchedOccurrences);
    }

    private List<AnalyzedText> RunDigestionAutomaton(List<UnmatchedTextOccurrence> allUnmatchedOccurrences)
    {
        // =================================================================================
        // Suffix Automaton Construction
        // =================================================================================
        var wordToId = new Dictionary<string, int>(StringComparer.Ordinal);
        var idToWord = new List<string>();
        var flattenedWordSequenceIdList = new List<int>();
        var indexToOccurrenceMap = new List<UnmatchedTextOccurrence>();
        int nextWordId = 0;
        int nextCorrelationId = -1;
        
        foreach (var occurrence in allUnmatchedOccurrences)
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
        // Extract Spans and Consolidate with Rich, Key-Based Contexts
        // =================================================================================
        var result = new List<AnalyzedText>();
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
        var wholeCounts = allUnmatchedOccurrences
            .GroupBy(s => s.Text)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);
        
        foreach (var (text, count) in wholeCounts)
        {
            if (!allMaximalSpans.ContainsKey(text))
                allMaximalSpans.Add(text, count);
        }
        
        foreach (var (spanText, count) in allMaximalSpans.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
        {
            if (string.IsNullOrWhiteSpace(spanText))
                continue;
        
            var subSpanContexts = new List<SubSpanContext>();
            var precedingSequencesWithKeys = new List<(List<GlyphInfo> Sequence, string Key)>();
            var followingSequencesWithKeys = new List<(List<GlyphInfo> Sequence, string Key)>();
        
            var spanTextWords = spanText.Trim().Split(' ');
            var allOccurrenceIndices = FindAllOccurrences(
                flattenedWordSequenceIdList,
                spanTextWords.Select(w =>  wordToId[w]).ToArray());
        
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
        
                var precedingSequence = new List<GlyphInfo>();
                var followingSequence = new List<GlyphInfo>();
        
                // Process PRECEDING sequences: nearest → farthest
                for (int i = wordStartIndexInSource - 1; i >= 0; i--)
                    precedingSequence.Add(new(originalSpanOccurrence.Words[i], null));
        
                for (int i = originalSpanOccurrence.UnmatchedTokenIndex - 1; i >= 0; i--)
                {
                    var token = originalSpanOccurrence.LineGlyphs[i];
                    Type type = token is Glyph ? token.Type : null;
                    precedingSequence.Add(new(token.CaptureValue, type));
                }
        
                if (precedingSequence.Any())
                    precedingSequencesWithKeys.Add((precedingSequence, originalSpanOccurrence.DocumentName));
                
                // Build FOLLOWING sequences: nearest → farthest
                int followingWordIndex = wordStartIndexInSource + spanTextWords.Length;
                for (int i = followingWordIndex; i < originalSpanOccurrence.Words.Length; i++)
                    followingSequence.Add(new(originalSpanOccurrence.Words[i], null));
        
                // Process FOLLOWING tokens nearest → farthest
                for (int i = originalSpanOccurrence.UnmatchedTokenIndex + 1; i < originalSpanOccurrence.LineGlyphs.Length; i++)
                {
                    var token = originalSpanOccurrence.LineGlyphs[i];
                    Type type = token is Glyph ? token.Type : null;
                    followingSequence.Add(new(token.CaptureValue, type));
                }
        
                if (followingSequence.Any())
                    followingSequencesWithKeys.Add((followingSequence, originalSpanOccurrence.DocumentName));
            }
        
            //Reverse collapsed text for PRECEDING so phrases read left→right (farthest→nearest)
            var precedingAdjacencyTree = BuildAdjacencyTree(precedingSequencesWithKeys, reverseCollapsedText: true);
            var followingAdjacencyTree = BuildAdjacencyTree(followingSequencesWithKeys, reverseCollapsedText: false);

            result.Add(new AnalyzedText(
                text: spanText,
                maximalSpanOccurrenceCount: count,
                occurrences: subSpanContexts,
                precedingAdjacencies: precedingAdjacencyTree,
                followingAdjacencies: followingAdjacencyTree,
                isLeftMaximal: ComputeIsLeftMaximal(subSpanContexts)
            ));
        }

        return result;
    }

    /// <summary>
    /// A span is left-maximal unless every occurrence shares one identical immediately-preceding
    /// word. An occurrence with no preceding word at all (it starts its source occurrence) is a
    /// boundary — a distinct context in its own right — so it always makes the span left-maximal,
    /// the same way reaching the end of an occurrence is vacuously right-maximal.
    /// </summary>
    private static bool ComputeIsLeftMaximal(List<SubSpanContext> occurrences)
    {
        string dominantPrecedingWord = null;
        bool hasDominantWord = false;

        foreach (var occurrence in occurrences)
        {
            if (occurrence.WordStartIndex == 0)
                return true;

            var precedingWord = occurrence.OriginalOccurrence.Words[occurrence.WordStartIndex - 1];

            if (!hasDominantWord)
            {
                dominantPrecedingWord = precedingWord;
                hasDominantWord = true;
            }
            else if (!string.Equals(dominantPrecedingWord, precedingWord, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds every fully-maximal (right- and left-maximal) corpus span that occurs as a contiguous
    /// word sequence somewhere within <paramref name="occurrence"/>, excluding spans whose only
    /// occurrences are in this same document (nothing external to point to). Sorted by word count
    /// descending then start index ascending — the stacking priority order, closest-to-text first.
    /// </summary>
    public List<EchoMatch> FindEchoes(UnmatchedTextOccurrence occurrence, int minWords, int minOccurrences)
    {
        var matches = new List<EchoMatch>();
        var words = occurrence.Words;

        foreach (var span in Spans)
        {
            if (!span.IsLeftMaximal) continue;
            if (span.WordCount < minWords) continue;
            if (span.MaximalSpanOccurrenceCount < minOccurrences) continue;

            var occurrencesElsewhere = span.TotalOccurrenceCount -
                span.OccurrencesPerDocument.GetValueOrDefault(occurrence.DocumentName, 0);

            // Never underline a span this document alone accounts for — nothing external to
            // point to. The badge itself shows the full total (this document included), so the
            // lowest number ever displayed is 2: at least one occurrence here, at least one
            // elsewhere for the span to have passed this check at all.
            if (occurrencesElsewhere < 1) continue;

            var spanWords = span.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (spanWords.Length == 0 || spanWords.Length > words.Length) continue;

            for (int i = 0; i <= words.Length - spanWords.Length; i++)
            {
                bool isMatch = true;
                for (int j = 0; j < spanWords.Length; j++)
                {
                    if (!string.Equals(words[i + j], spanWords[j], StringComparison.Ordinal))
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                    matches.Add(new EchoMatch(span, i, span.TotalOccurrenceCount));
            }
        }

        return matches
            .OrderByDescending(m => m.Span.WordCount)
            .ThenBy(m => m.WordStartIndex)
            .ToList();
    }

    /// <summary>
    /// Packs echoes into the fewest non-overlapping lanes (rows), greedily, by word range —
    /// classic interval-graph coloring: sort by start (ties broken by longer-first), then place
    /// each echo in the first lane whose last-placed echo already ends at or before this one's
    /// start, opening a new lane only when none does. This is a decluttering choice, not a
    /// priority ranking — two short, disjoint echoes sharing one lane is preferred over spending
    /// a whole extra row on each, so lane order no longer reflects occurrence count or length.
    /// Static and side-effect-free (no dependency on this corpus instance's own Spans) so both the
    /// per-occurrence renderer (SpanView) and the per-line space reservation below
    /// (GetMaxEchoLaneCount) can share one packing without duplicating it.
    /// </summary>
    public static List<List<EchoMatch>> PackEchoLanes(List<EchoMatch> echoes)
    {
        var sorted = echoes
            .OrderBy(e => e.WordStartIndex)
            .ThenByDescending(e => e.Span.WordCount)
            .ToList();

        var laneEnds = new List<int>();
        var lanes = new List<List<EchoMatch>>();

        foreach (var echo in sorted)
        {
            int start = echo.WordStartIndex;
            int end = echo.WordStartIndex + echo.Span.WordCount;

            int lane = laneEnds.FindIndex(laneEnd => laneEnd <= start);
            if (lane == -1)
            {
                lane = lanes.Count;
                lanes.Add([]);
                laneEnds.Add(0);
            }

            lanes[lane].Add(echo);
            laneEnds[lane] = end;
        }

        return lanes;
    }

    /// <summary>
    /// The most lanes any single UnmatchedString occurrence on this line would need to render its
    /// own echoes — the echo-underline equivalent of a line's deepest capture-trace depth. Used to
    /// make sure a line reserves enough vertical room that a dense stack of echo underlines
    /// doesn't run into the next physically-wrapped line of text, the same way capture depth
    /// already does for colored underlines (see CaptureTraceDisplayContext.MaxEffectiveDepth,
    /// which takes the max of that and this).
    /// </summary>
    public int GetMaxEchoLaneCount(ProcessedLine line, int minWords, int minOccurrences)
    {
        int max = 0;

        foreach (var occurrence in line.UnmatchedTextOccurrences)
        {
            var echoes = FindEchoes(occurrence, minWords, minOccurrences);
            if (echoes.Count == 0) continue;

            var laneCount = PackEchoLanes(echoes).Count;
            if (laneCount > max) max = laneCount;
        }

        return max;
    }

    /// <summary>
    /// Builds an adjacency tree from token sequences. It identifies any non-branching
    /// path and collapses it into a single AdjacencyNode.
    /// </summary>
    private List<AdjacencyNode> BuildAdjacencyTree(
        List<(List<GlyphInfo> Sequence, string Key)> sequencesWithKeys,
        bool reverseCollapsedText)
    {
        if (sequencesWithKeys == null || !sequencesWithKeys.Any(x => x.Sequence.Any()))
            return [];

        var nodeGroups = sequencesWithKeys.Where(x => x.Sequence.Any()).GroupBy(x => x.Sequence.First());
        var nodes = new List<AdjacencyNode>();

        foreach (var group in nodeGroups)
        {
            var initialToken = group.Key;
            var sourceOccurrenceDocumentNames = group.Select(g => g.Key).Distinct().ToList();

            // Collect the collapsed linear segment
            var segmentsToCollapse = new List<(string Text, HexPalette Palette)>
            {
                (initialToken.Text, initialToken.GlyphType is null ? null : DeterministicPalette.TypePaletteSet[initialToken.GlyphType])
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

                segmentsToCollapse.Add((nextToken.Text, nextToken.GlyphType is null ? null : DeterministicPalette.TypePaletteSet[nextToken.GlyphType]));

                // consume one token
                remainingSequences = remainingSequences.Select(x => (Sequence: x.Sequence.Skip(1).ToList(), x.Key)).ToList();
            }

            // Recurse on what remains after collapsing
            var children = BuildAdjacencyTree(remainingSequences, reverseCollapsedText);

            // Build the final combined text and palette map.
            // For PRECEDING trees we reverse the collapsed segment order so the phrase reads farthest→nearest.
            var finalTextBuilder = new StringBuilder();
            var palettes = new Dictionary<int, HexPalette>();

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
                sourceOccurrenceDocumentNames: sourceOccurrenceDocumentNames,
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