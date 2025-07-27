namespace MTGPlexer.TokenAnalysis;

public record CardDigester
{
    public List<CardDigest> DigestedCards { get; }
    public Dictionary<Type, int> TokenCounts { get; } = [];
    public List<UnmatchedSpanCount> UnmatchedSpanCounts { get; }

    public CardDigester(int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        var cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);
        DigestedCards = cards.Select(x => new CardDigest(x)).ToList();
        DigestedCards.ForEach(x => TokenCounts.CombineDictionaryCounts(x.TokenCounts));
        UnmatchedSpanCounts = SpanOccurrenceCounter.GetUnmatchedSpanCounts(DigestedCards);
    }

    ///// <summary>
    ///// A highly performant method to generate all word-based substrings from a collection of strings.
    ///// </summary>
    ///// <param name="DigestedCards">Your collection of objects containing the strings.</param>
    //public List<UnmatchedSpanCount> GetCombinatoricSpans()
    //{
    //    var combinatoricSpans = new Dictionary<string, int>();
    //
    //    var lengthOrderedUnmatchedSpans = DigestedCards
    //        .SelectMany(x => x.UnmatchedSpans)
    //        .OrderByDescending(x => x.Length)
    //        .ToList();
    //
    //    string currentWholeSpan;
    //    int lastOccurrenceCount;
    //
    //    foreach (var wholeSpan in lengthOrderedUnmatchedSpans)
    //    {
    //        currentWholeSpan = wholeSpan;
    //
    //        // Step 1: Find the start and end index of each word once.
    //        var wordBoundaries = GetWordBoundaries(wholeSpan);
    //
    //        // Step 2: Generate substrings from the calculated boundaries.
    //        for (int i = 0; i < wordBoundaries.Count; i++)
    //        {
    //            for (int j = i; j < wordBoundaries.Count; j++)
    //            {
    //                // Get the start of the first word and the end of the last word.
    //                int start = wordBoundaries[i].start;
    //                int end = wordBoundaries[j].end;
    //
    //                // Create the substring directly from the original string.
    //                // This is the only string allocation in the loop.
    //                string subSpan = wholeSpan.Substring(start, end - start);
    //
    //                // Efficiently update the dictionary count.
    //                if (!combinatoricSpans.TryGetValue(subSpan, out int currentCount))
    //                    currentCount = 0;
    //
    //                combinatoricSpans[subSpan] = currentCount + 1;
    //            }
    //        }
    //    }
    //
    //    // Local helper
    //    void AddToDictionaryIfMaximal(string span, bool isWholeSpan = false)
    //    {
    //        if (!forceAdd)
    //    }
    //
    //    return combinatoricSpans.Select(x => new UnmatchedSpanCount(x.Key, x.Value)).ToList();
    //}
    //
    ///// <summary>
    ///// Helper method to find the start and end character index of each word in a string.
    ///// Handles leading, trailing, and multiple consecutive spaces.
    ///// </summary>
    //private static List<(int start, int end)> GetWordBoundaries(string text)
    //{
    //    var boundaries = new List<(int, int)>();
    //    int startIndex = -1;
    //
    //    for (int i = 0; i < text.Length; i++)
    //    {
    //        if (!char.IsWhiteSpace(text[i]))
    //        {
    //            // If we are not in a word, mark the start of a new one.
    //            if (startIndex == -1)
    //                startIndex = i;
    //        }
    //        else
    //        {
    //            // If we were in a word and hit a space, mark the end.
    //            if (startIndex != -1)
    //            {
    //                boundaries.Add((startIndex, i));
    //                startIndex = -1; // Reset for the next word.
    //            }
    //        }
    //    }
    //
    //    // If the string doesn't end with a space, add the last word.
    //    if (startIndex != -1)
    //        boundaries.Add((startIndex, text.Length));
    //
    //    return boundaries;
    //}
}