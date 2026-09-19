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
        // Read off the type rather than its RegexGraph: this runs eagerly over every candidate, and
        // the dependent types added above aren't guaranteed to have a graph registered (under
        // IsolateForTesting they're discovered unfiltered while the graphs are built only for the
        // isolated closure). The loop below only ever indexes a graph for a type it actually reaches.
        bool anyCandidateAllowsPartialSegment = filteredTypes
            .Any(x => x.IsDefined(typeof(AllowPartialSegmentMatchAttribute)));

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

                // The rule is "cover a whole number of segments, at least one" - so a type is offered each
                // successive clause boundary as a candidate end, shortest first, and must fill whichever
                // one it takes. Only a type that declares a clause break of its own (SpansClauses) is
                // offered more than the first: everything else is capped at one segment, which is what
                // stops a greedy wildcard from swallowing clauses it never said it wanted.
                // MustMatchWholeLine and the exempt types skip all this and run against the full scope, as
                // before - the former still has to fill it (RegexGraph applies that itself), the latter
                // still may end partway through it.
                foreach (var candidateEnd in GetCandidateEnds(sourceText, endIndex, segmentEndIndex, mustConsumeSegment, rootNode.SpansClauses))
                {
                    if (!rootNode.TryMatch(sourceText, currentIndex, candidateEnd, out var token, mustConsumeSegment))
                        continue;

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

                if (matched)
                    break;
            }

            if (!matched)
            {
                // Nothing matched here because the cursor is sitting on a clause-separating period. That
                // period is modeled punctuation, not text still awaiting a Glyph, so it gets a token of its
                // own rather than being folded into the surrounding unmatched span - see ClauseBreak.
                if (currentIndex == segmentEndIndex && currentIndex < endIndex && sourceText[currentIndex] == '.')
                {
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);
                    tokens.Add(new ClauseBreak(sourceText, currentIndex, 1));

                    // Step past the period and the whitespace trailing it, so the next clause opens on its
                    // first real character rather than on a space that would read as unmatched text.
                    currentIndex++;

                    while (currentIndex < endIndex && sourceText[currentIndex] == ' ')
                        currentIndex++;

                    continue;
                }

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

                // ...but never past this clause's terminating period. A period is a hard delimiter, so
                // stopping on it is what keeps the run of unmatched text this ratchet is accumulating from
                // spanning one - the next pass then emits it as its own ClauseBreak. Without this, whether
                // a period became a token or got swallowed into unmatched text would depend on the
                // accident of whether the preceding clause happened to match.
                if (currentIndex > segmentEndIndex && segmentEndIndex < endIndex)
                    currentIndex = segmentEndIndex;
            }
        }

        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, endIndex);

        return tokens;
    }

    /// <summary>
    /// The scope ends a type may be matched against at the current position, in the order they should be
    /// tried. A type not held to the whole-segment rule gets the full scope and nothing else (it either
    /// fills the line or may end partway, both of which <see cref="RegexGraph.TryMatch"/> decides on its
    /// own). A type held to it gets the first clause boundary, and - only if it declares a clause break of
    /// its own - each later one in turn, shortest first, so it settles on the fewest clauses that satisfy
    /// it rather than the most.
    /// </summary>
    static IEnumerable<int> GetCandidateEnds(
        string sourceText, int endIndex, int segmentEndIndex, bool mustConsumeSegment, bool spansClauses)
    {
        if (!mustConsumeSegment)
        {
            yield return endIndex;
            yield break;
        }

        yield return segmentEndIndex;

        if (!spansClauses)
            yield break;

        int candidateEnd = segmentEndIndex;

        // Each step moves past the period just offered and out to the next boundary, so the sequence
        // strictly increases and terminates at endIndex.
        while (candidateEnd < endIndex)
        {
            candidateEnd = FindSegmentEnd(sourceText, candidateEnd + 1, endIndex);
            yield return candidateEnd;
        }
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
