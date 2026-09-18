namespace Glyphotype.Tokenizers;

public class Tokenizer
{
    private readonly List<Type> _orderedTopLevelTypes;
    private readonly List<Type> _dependentTypes;
    private readonly bool _allowPartialSegmentMatches;
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    /// <param name="allowPartialSegmentMatches">
    /// The default for <see cref="Tokenize"/>'s parameter of the same name, supplied from
    /// <see cref="GlobalSettings.AllowPartialSegmentMatches"/> by
    /// <see cref="GlyphTypeRegistry"/> when it builds <see cref="GlyphTypeRegistry.ClassTokenizer"/>.
    /// </param>
    public Tokenizer(List<Type> orderedTopLevelTypes, List<Type> dependentTypes, bool allowPartialSegmentMatches)
    {
        _orderedTopLevelTypes = orderedTopLevelTypes;
        _dependentTypes = dependentTypes;
        _allowPartialSegmentMatches = allowPartialSegmentMatches;
    }

    /// <param name="allowPartialSegmentMatches">
    /// Overrides this Tokenizer's configured <see cref="GlobalSettings.AllowPartialSegmentMatches"/>
    /// default for this one call. Null (the usual case) uses that default. Passed explicitly by callers
    /// that aren't tokenizing a line at all but resolving an already-matched sub-capture - see
    /// <see cref="DynamicGlyphNode.TryHydrate"/> - where "consume the whole segment" is both meaningless
    /// (the capture is a fragment of one segment, not a segment) and actively harmful (that resolution
    /// depends on being handed the shorter prefix a whole-segment rule would reject outright).
    /// </param>
    public List<CaptureUnit> Tokenize(
        string sourceText,
        int? scopeStart = null, int?
        scopeEnd = null,
        Type scopeToType = null,
        bool includeDependentTypes = false,
        bool? allowPartialSegmentMatches = null)
    {
        if (string.IsNullOrEmpty(sourceText))
            throw new Exception("Source text may not be null or empty");

        var tokens = new List<CaptureUnit>();
        int currentIndex = scopeStart ?? 0;
        int endIndex = scopeEnd ?? sourceText.Length;
        int unmatchedStartIndex = -1;

        // Fixed anchor for the scope this call is tokenizing, used below to gate MustMatchWholeLine
        // types: currentIndex advances as tokens get committed or unmatched text gets skipped, but a
        // MustMatchWholeLine type is only a valid candidate on the very first attempt at this scope's own
        // start - once anything (a token or a ratchet skip) has consumed part of the scope, no match
        // starting after that point could still be "the whole line" by itself.
        int scopeStartIndex = currentIndex;

        // Whether a top-level type may match only part of a segment (see GlobalSettings). When it may
        // not, a type is only a candidate at a segment's own start and has to consume that segment whole,
        // gated by the segment bookkeeping maintained across the loop below.
        bool requireWholeSegments = !(allowPartialSegmentMatches ?? _allowPartialSegmentMatches);

        var candidateTypes = _orderedTopLevelTypes.ToList();

        if (includeDependentTypes)
            candidateTypes.AddRange(_dependentTypes);

        // Pre-filter types to avoid repeating logic inside the while loop
        var filteredTypes =
            (scopeToType != null && scopeToType != typeof(Glyph)) ? candidateTypes.Where(x => x.IsAssignableTo(scopeToType)).ToList()
            : candidateTypes;

        // AllowPartialSegmentMatch is the only thing that keeps a type a candidate partway into a segment
        // - a MustMatchWholeLine type is even stricter than the requirement, so it's already ruled out
        // anywhere past the scope start. With none of them in the candidate set, a segment that didn't
        // match at its own start can't match anywhere within itself, which the ratchet below exploits.
        bool anyCandidateAllowsPartialSegment = filteredTypes
            .Any(x => GlyphTypeRegistry.RegexGraphs[x].AllowsPartialSegmentMatch);

        int segmentStartIndex = scopeStartIndex;
        int segmentEndIndex = FindSegmentEnd(sourceText, segmentStartIndex, endIndex);

        while (currentIndex < endIndex)
        {
            // Open the next segment once the cursor has moved past this one's terminating period.
            // Strictly greater, not >=: landing exactly on the period means the period itself is still
            // unconsumed, and it's no more the start of the next segment than it is part of this one.
            if (currentIndex > segmentEndIndex)
            {
                segmentStartIndex = currentIndex;
                segmentEndIndex = FindSegmentEnd(sourceText, currentIndex, endIndex);
            }

            bool matched = false;
            bool atSegmentStart = currentIndex == segmentStartIndex;

            foreach (var type in filteredTypes)
            {
                var rootNode = GlyphTypeRegistry.RegexGraphs[type];

                if (rootNode.MustMatchWholeLine && currentIndex != scopeStartIndex)
                    continue;

                // A whole-segment candidate that isn't sitting at a segment's start can't satisfy the
                // requirement no matter what it would match, so don't pay for the match at all.
                bool mustConsumeSegment = requireWholeSegments
                    && !rootNode.MustMatchWholeLine
                    && !rootNode.AllowsPartialSegmentMatch;

                if (mustConsumeSegment && !atSegmentStart)
                    continue;

                // A whole-segment match is just the whole-scope rule applied to a segment-sized scope:
                // cap the scope at the segment end so a greedy pattern can't reach past the period, then
                // require the match to fill it. MustMatchWholeLine keeps the full scope - it demands the
                // entire line, which is strictly more than any one segment of it.
                int typeEndIndex = mustConsumeSegment ? segmentEndIndex : endIndex;

                if (rootNode.TryMatch(sourceText, currentIndex, typeEndIndex, out var token, mustConsumeSegment))
                {
                    // --- COMMIT PHASE ---
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                    tokens.Add(token);

                    // Advance by the length of the matched token
                    // Assuming Glyph contains the length or the raw text
                    currentIndex += token.CaptureValue.Length;

                    unmatchedStartIndex = -1;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                if (unmatchedStartIndex == -1)
                    unmatchedStartIndex = currentIndex;

                // Nothing left in the candidate set can match before this segment ends (see
                // anyCandidateAllowsPartialSegment), so skip the rest of it in one step rather than
                // ratcheting a word at a time. Purely an optimization: that span is unmatched either way
                // and gets flushed as the very same single UnmatchedString.
                bool skipRestOfSegment = requireWholeSegments
                    && !anyCandidateAllowsPartialSegment
                    && !atSegmentStart
                    && currentIndex < segmentEndIndex;

                if (skipRestOfSegment)
                {
                    currentIndex = segmentEndIndex;
                    continue;
                }

                // Ratchet logic: skip to the next space
                int nextSpaceIndex = sourceText.IndexOf(' ', currentIndex);

                if (nextSpaceIndex == -1 || nextSpaceIndex >= endIndex)
                    currentIndex = endIndex;
                else
                    currentIndex = nextSpaceIndex + 1;
            }
        }

        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, endIndex);

        return tokens;
    }

    /// <summary>
    /// The exclusive end of the segment starting at <paramref name="fromIndex"/>: the next period at or
    /// after it, or <paramref name="endIndex"/> if the scope runs out first. The period itself is never
    /// part of the segment - a whole-segment match ends immediately before it, leaving the period to the
    /// ordinary ratchet on a later pass, exactly as it was before segments existed.
    /// </summary>
    static int FindSegmentEnd(string sourceText, int fromIndex, int endIndex)
    {
        if (fromIndex >= endIndex)
            return endIndex;

        int periodIndex = sourceText.IndexOf('.', fromIndex);

        return periodIndex == -1 || periodIndex >= endIndex ? endIndex : periodIndex;
    }

    private void FlushUnmatched(string sourceText, List<CaptureUnit> tokens, ref int unmatchedStartIndex, int flushUntilIndex)
    {
        if (unmatchedStartIndex == -1 || unmatchedStartIndex >= flushUntilIndex)
        {
            unmatchedStartIndex = -1;
            return;
        }

        int length = flushUntilIndex - unmatchedStartIndex;

        if (length <= 0)
        {
            unmatchedStartIndex = -1;
            return;
        }

        UnmatchedString defualtUnmatchedString = new(sourceText, unmatchedStartIndex, length);
        tokens.Add(defualtUnmatchedString);

        unmatchedStartIndex = -1;
    }
}
