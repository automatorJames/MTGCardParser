using System.Collections.Concurrent;

namespace Glyphotype.Tokenizers;

public class Tokenizer
{
    private readonly List<Type> _orderedTopLevelTypes;
    private readonly List<Type> _dependentTypes;

    /// <summary>The types one <see cref="Tokenize"/> call tries, as their graphs in the order tried.</summary>
    sealed record CandidateSet(RegexGraph[] Graphs);

    /// <summary>
    /// <see cref="CandidateSet"/>s by (scope type, dependents included?) - the only inputs they depend on, and a
    /// handful of distinct combinations in practice. Resolved once rather than per call: every line of a corpus
    /// is a call, and resolving the set (filtering types, looking up graphs) would otherwise cost more than
    /// tokenizing the line.
    /// </summary>
    readonly ConcurrentDictionary<(Type ScopeToType, bool IncludeDependentTypes), CandidateSet> _candidateSets = new();

    public Tokenizer(List<Type> orderedTopLevelTypes, List<Type> dependentTypes)
    {
        _orderedTopLevelTypes = orderedTopLevelTypes;
        _dependentTypes = dependentTypes;
    }

    /// <summary>
    /// Tokenizes <paramref name="sourceText"/> clause by clause: a type matches only at a clause's start, and only by
    /// consuming that clause whole (or several, if it declares a clause break of its own - see
    /// <see cref="RegexGraph.SpansClauses"/>). A clause nothing consumes whole is unmatched text, so an
    /// incompletely-modeled clause shows as such rather than hiding behind a match of part of it.
    /// </summary>
    /// <param name="allowPartialClauseMatches">
    /// Lifts the whole-clause rule for this one call, so a type may start anywhere and end at any word boundary.
    /// Only for callers that aren't tokenizing a line at all but resolving an already-matched sub-capture - see
    /// <see cref="DynamicGlyphNode.TryHydrate"/> - where "consume the whole clause" is both meaningless
    /// (the capture is a fragment of one clause, not a clause) and actively harmful (that resolution
    /// depends on being handed the shorter prefix a whole-clause rule would reject outright).
    /// </param>
    public List<CaptureUnit> Tokenize(
        string sourceText,
        int? scopeStart = null, int?
        scopeEnd = null,
        Type scopeToType = null,
        bool includeDependentTypes = false,
        bool allowPartialClauseMatches = false)
    {
        if (string.IsNullOrEmpty(sourceText))
            throw new Exception("Source text may not be null or empty");

        var tokens = new List<CaptureUnit>();
        int currentIndex = scopeStart ?? 0;
        int endIndex = scopeEnd ?? sourceText.Length;
        int unmatchedStartIndex = -1;

        // Held to the whole-clause rule, a type is only a candidate at a clause's own start and has to consume
        // that clause whole, gated by the clause bookkeeping maintained across the loop below.
        bool requireWholeClauses = !allowPartialClauseMatches;

        var candidates = GetCandidateSet(scopeToType, includeDependentTypes);

        int clauseStartIndex = currentIndex;
        int clauseEndIndex = FindClauseEnd(sourceText, clauseStartIndex, endIndex);

        while (currentIndex < endIndex)
        {
            // Open the next clause once the cursor has moved past this one's terminating period.
            // Strictly greater, not >=: landing exactly on the period means the period itself is still
            // unconsumed, and it's no more the start of the next clause than it is part of this one.
            if (currentIndex > clauseEndIndex)
            {
                clauseStartIndex = currentIndex;
                clauseEndIndex = FindClauseEnd(sourceText, currentIndex, endIndex);
            }

            bool matched = false;
            bool atClauseStart = currentIndex == clauseStartIndex;

            // Held to the whole-clause rule, no type is offered a match partway through a clause (see below).
            if (!requireWholeClauses || atClauseStart)
                TokenizationTrace.NoteAttempt(sourceText, currentIndex, endIndex, clauseEndIndex, requireWholeClauses, scopeToType, includeDependentTypes);

            foreach (var rootNode in candidates.Graphs)
            {
                // Can't match here at all, so skip it without paying for a regex call (see StartCharSet).
                if (!rootNode.StartChars.CanStartWith(sourceText[currentIndex]))
                    continue;

                // A whole-clause candidate that isn't sitting at a clause's start can't satisfy the
                // requirement no matter what it would match, so don't pay for the match at all.
                if (requireWholeClauses && !atClauseStart)
                    continue;

                // The rule is "cover a whole number of clauses, at least one" - so a type is offered each
                // successive clause boundary as a candidate end, shortest first, and must fill whichever
                // one it takes. Only a type that declares a clause break of its own (SpansClauses) is
                // offered more than the first: everything else is capped at one clause, which is what
                // stops a greedy wildcard from swallowing clauses it never said it wanted. With the rule lifted,
                // a type skips all this and runs against the full scope, where it may end partway through.
                foreach (var candidateEnd in GetCandidateEnds(sourceText, endIndex, clauseEndIndex, requireWholeClauses, rootNode.SpansClauses))
                {
                    if (!rootNode.TryMatch(sourceText, currentIndex, candidateEnd, out var token, requireWholeClauses, tokenizer: this))
                        continue;

                    // --- COMMIT PHASE ---
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                    tokens.Add(token);

                    // Advance by the length of the matched token
                    // Assuming Glyph contains the length or the raw text
                    currentIndex += token.CaptureValue.Length;

                    // Step past the whitespace separating this token from whatever follows, just as after a
                    // ClauseBreak - otherwise an unmatched remainder would open with that space (and a space
                    // between two consecutive tokens would be flushed as unmatched text of its own).
                    while (currentIndex < endIndex && sourceText[currentIndex] == ' ')
                        currentIndex++;

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
                if (currentIndex == clauseEndIndex && currentIndex < endIndex && sourceText[currentIndex] == ClauseBreak.Period)
                {
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                    // A closing parenthesis or quote straight after the period ends the same clause the period
                    // does ("the dog sleeps (the cat eats fish.)"), so it belongs to the break.
                    int breakLength = ClauseBreak.LengthAt(sourceText, currentIndex, endIndex);

                    tokens.Add(new ClauseBreak(sourceText, currentIndex, breakLength));

                    // Step past the break and the whitespace trailing it, so the next clause opens on its
                    // first real character rather than on a space that would read as unmatched text.
                    currentIndex += breakLength;

                    while (currentIndex < endIndex && sourceText[currentIndex] == ' ')
                        currentIndex++;

                    continue;
                }

                if (unmatchedStartIndex == -1)
                    unmatchedStartIndex = currentIndex;

                // Held to the whole-clause rule, nothing can match before this clause ends, so skip the rest
                // of it in one step rather than ratcheting a word at a time. Purely an optimization: that span
                // is unmatched either way and gets flushed as the very same single UnmatchedString.
                bool skipRestOfClause = requireWholeClauses
                    && !atClauseStart
                    && currentIndex < clauseEndIndex;

                if (skipRestOfClause)
                {
                    currentIndex = clauseEndIndex;
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
                if (currentIndex > clauseEndIndex && clauseEndIndex < endIndex)
                    currentIndex = clauseEndIndex;
            }
        }

        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, endIndex);

        return tokens;
    }

    CandidateSet GetCandidateSet(Type scopeToType, bool includeDependentTypes) =>
        _candidateSets.GetOrAdd((scopeToType ?? typeof(Glyph), includeDependentTypes), key =>
        {
            var types = includeDependentTypes ? _orderedTopLevelTypes.Concat(_dependentTypes) : _orderedTopLevelTypes;

            if (key.ScopeToType != typeof(Glyph))
                types = types.Where(x => x.IsAssignableTo(key.ScopeToType));

            var graphs = types.Select(GlyphTypeCache.GetRegexGraph).ToArray();

            return new(graphs);
        });

    /// <summary>
    /// The scope ends a type may be matched against at the current position, in the order they should be
    /// tried. With the whole-clause rule lifted, a type gets the full scope and nothing else (it may end
    /// partway, which <see cref="RegexGraph.TryMatch"/> decides on its own). Held to it, a type gets the first
    /// clause boundary, and - only if it declares a clause break of its own - each later one in turn,
    /// shortest first, so it settles on the fewest clauses that satisfy it rather than the most.
    /// </summary>
    internal static IEnumerable<int> GetCandidateEnds(
        string sourceText, int endIndex, int clauseEndIndex, bool mustConsumeClause, bool spansClauses)
    {
        if (!mustConsumeClause)
        {
            yield return endIndex;
            yield break;
        }

        yield return clauseEndIndex;

        if (!spansClauses)
            yield break;

        int candidateEnd = clauseEndIndex;

        // Each step moves past the period just offered and out to the next boundary, so the sequence
        // strictly increases and terminates at endIndex.
        while (candidateEnd < endIndex)
        {
            candidateEnd = FindClauseEnd(sourceText, candidateEnd + 1, endIndex);
            yield return candidateEnd;
        }
    }

    /// <summary>
    /// The exclusive end of the clause starting at <paramref name="fromIndex"/>: the next period at or
    /// after it, or <paramref name="endIndex"/> if the scope runs out first. The period itself is never
    /// part of the clause - a whole-clause match ends immediately before it, leaving the period to the
    /// ordinary ratchet on a later pass, which emits it as a <see cref="ClauseBreak"/>.
    /// </summary>
    static int FindClauseEnd(string sourceText, int fromIndex, int endIndex)
    {
        if (fromIndex >= endIndex)
            return endIndex;

        int periodIndex = sourceText.IndexOf(ClauseBreak.Period, fromIndex);

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
