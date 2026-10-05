using System.Collections.Concurrent;

namespace Glyphotype.Tokenizers;

public class Tokenizer
{
    private readonly List<Type> _orderedTopLevelTypes;
    private readonly List<Type> _dependentTypes;
    private readonly bool _allowPartialClauseMatches;

    /// <summary>
    /// The types one <see cref="Tokenize"/> call tries, as their graphs in the order tried - and whether any of
    /// them may match partway into a clause (see <see cref="RegexGraph.AllowsPartialClauseMatch"/>).
    /// </summary>
    sealed record CandidateSet(RegexGraph[] Graphs, bool AnyAllowsPartialClause);

    /// <summary>
    /// <see cref="CandidateSet"/>s by (scope type, dependents included?) - the only inputs they depend on, and a
    /// handful of distinct combinations in practice. Resolved once rather than per call: every line of a corpus
    /// is a call, and resolving the set (filtering types, looking up graphs) would otherwise cost more than
    /// tokenizing the line.
    /// </summary>
    readonly ConcurrentDictionary<(Type ScopeToType, bool IncludeDependentTypes), CandidateSet> _candidateSets = new();

    /// <param name="allowPartialClauseMatches">
    /// The default for <see cref="Tokenize"/>'s parameter of the same name, supplied by the
    /// <see cref="GlyphGrammar"/> that builds this Tokenizer (see <see cref="GlyphGrammar.AllowPartialClauseMatches"/>).
    /// </param>
    public Tokenizer(List<Type> orderedTopLevelTypes, List<Type> dependentTypes, bool allowPartialClauseMatches)
    {
        _orderedTopLevelTypes = orderedTopLevelTypes;
        _dependentTypes = dependentTypes;
        _allowPartialClauseMatches = allowPartialClauseMatches;
    }

    /// <param name="allowPartialClauseMatches">
    /// Overrides this Tokenizer's configured <see cref="GlobalSettings.AllowPartialClauseMatches"/>
    /// default for this one call. Null (the usual case) uses that default. Passed explicitly by callers
    /// that aren't tokenizing a line at all but resolving an already-matched sub-capture - see
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
        bool? allowPartialClauseMatches = null)
    {
        if (string.IsNullOrEmpty(sourceText))
            throw new Exception("Source text may not be null or empty");

        return Tokenize(
            sourceText, scopeStart ?? 0, scopeEnd ?? sourceText.Length, scopeToType, includeDependentTypes,
            allowPartialClauseMatches ?? _allowPartialClauseMatches, depth: 0);
    }

    /// <summary>
    /// <see cref="Tokenize(string, int?, int?, Type, bool, bool?)"/> from <paramref name="startIndex"/> to
    /// <paramref name="endIndex"/>, which sit inside <paramref name="depth"/> enclosures (see <see cref="Enclosures"/>):
    /// the inside of one no glyph matched whole is tokenized by a call of its own, one deeper, its periods ending the
    /// clauses nested there.
    /// <para>
    /// An enclosure is also where a whole-clause match may stop or start - before it, after it, or exactly around
    /// it (see <see cref="IsSegmentStart"/> and <see cref="GetCandidateEnds"/>) - so "flying (this creature can't be
    /// blocked.)" can be a keyword glyph and a reminder-text glyph, while a glyph whose nibs take in an enclosure
    /// (<c>"has", Prop(Ability)</c> over a quoted ability) may still span it. Never part of one: every match is
    /// balanced (see <see cref="Enclosures.IsBalanced"/>).
    /// </para>
    /// </summary>
    List<CaptureUnit> Tokenize(
        string sourceText, int startIndex, int endIndex, Type scopeToType, bool includeDependentTypes, bool allowPartialClauseMatches, int depth)
    {
        var tokens = new List<CaptureUnit>();
        int currentIndex = startIndex;
        int unmatchedStartIndex = -1;

        // What the regexes run against: the text with its enclosed periods marked, so only a period ending a clause
        // of this scope reads as one (see Enclosures.MatchText). Captures are still read from sourceText.
        var enclosures = Enclosures.Scan(sourceText, startIndex, endIndex);
        var matchText = enclosures.MatchText;

        // Whether a top-level type may match only part of a clause (see GlobalSettings). When it may
        // not, a type is only a candidate at a clause's own start and has to consume that clause whole,
        // gated by the clause bookkeeping maintained across the loop below.
        bool requireWholeClauses = !allowPartialClauseMatches;

        var candidates = GetCandidateSet(scopeToType, includeDependentTypes);

        // AllowPartialClauseMatch is the only thing that keeps a type a candidate partway into a clause.
        // With none of them in the candidate set, a clause that didn't match at its own start can't match
        // anywhere within itself, which the ratchet below exploits.
        bool anyCandidateAllowsPartialClause = candidates.AnyAllowsPartialClause;

        int clauseStartIndex = currentIndex;
        int clauseEndIndex = FindClauseEnd(matchText, clauseStartIndex, endIndex);

        while (currentIndex < endIndex)
        {
            // Open the next clause once the cursor has moved past this one's terminating period.
            // Strictly greater, not >=: landing exactly on the period means the period itself is still
            // unconsumed, and it's no more the start of the next clause than it is part of this one.
            if (currentIndex > clauseEndIndex)
            {
                clauseStartIndex = currentIndex;
                clauseEndIndex = FindClauseEnd(matchText, currentIndex, endIndex);
            }

            bool matched = false;
            bool atSegmentStart = currentIndex == clauseStartIndex || IsSegmentStart(sourceText, enclosures, currentIndex);

            foreach (var rootNode in candidates.Graphs)
            {
                // Can't match here at all, so skip it without paying for a regex call (see StartCharSet).
                if (!rootNode.StartChars.CanStartWith(matchText[currentIndex]))
                    continue;

                // A whole-clause candidate that isn't sitting at a clause's start (or an enclosure's edge) can't satisfy the
                // requirement no matter what it would match, so don't pay for the match at all.
                bool mustConsumeClause = requireWholeClauses && !rootNode.AllowsPartialClauseMatch;

                if (mustConsumeClause && !atSegmentStart)
                    continue;

                // The rule is "cover a whole number of clauses, at least one" - so a type is offered each
                // successive clause boundary as a candidate end, shortest first, and must fill whichever
                // one it takes. Only a type that declares a clause break of its own (SpansClauses) is
                // offered more than the first: everything else is capped at one clause, which is what
                // stops a greedy wildcard from swallowing clauses it never said it wanted. Within the clause, an
                // enclosure's edges are boundaries too, offered first. An exempt type skips all this and runs
                // against the full scope, where it may end partway through.
                foreach (var candidateEnd in GetCandidateEnds(sourceText, matchText, enclosures, currentIndex, endIndex, clauseEndIndex, mustConsumeClause, rootNode.SpansClauses))
                {
                    if (!rootNode.TryMatch(sourceText, currentIndex, candidateEnd, out var token, mustConsumeClause, tokenizer: this, matchText: matchText))
                        continue;

                    // Part of an enclosure is no match: only one exempt from the whole-clause rule could end inside one.
                    if (!enclosures.IsBalanced(currentIndex, currentIndex + token.CaptureValue.Length))
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

                    // A stray closing parenthesis or quote straight after the period - one closing no enclosure -
                    // belongs to the break, rather than opening the next clause as text. One opening an enclosure
                    // doesn't.
                    int breakLength = ClauseBreak.LengthAt(sourceText, currentIndex, endIndex);

                    while (breakLength > 1 && enclosures.Outermost.Any(x => x.Open > currentIndex && x.Open < currentIndex + breakLength))
                        breakLength--;

                    tokens.Add(new ClauseBreak(sourceText, currentIndex, breakLength, depth));

                    // Step past the break and the whitespace trailing it, so the next clause opens on its
                    // first real character rather than on a space that would read as unmatched text.
                    currentIndex += breakLength;

                    while (currentIndex < endIndex && sourceText[currentIndex] == ' ')
                        currentIndex++;

                    continue;
                }

                // Nothing matched an enclosure starting here, whole or with what follows it: its inside is text of
                // its own, nested in this clause, and is tokenized as such - its delimiters structure, not text.
                if (enclosures.TryGetOpeningAt(currentIndex, out var enclosure))
                {
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);
                    tokens.Add(new EnclosureMark(sourceText, enclosure.Open, depth));

                    int innerStart = enclosure.Open + 1;
                    int innerEnd = enclosure.Close;

                    while (innerStart < innerEnd && sourceText[innerStart] == ' ')
                        innerStart++;

                    while (innerEnd > innerStart && sourceText[innerEnd - 1] == ' ')
                        innerEnd--;

                    if (innerStart < innerEnd)
                        tokens.AddRange(Tokenize(sourceText, innerStart, innerEnd, scopeToType, includeDependentTypes, allowPartialClauseMatches, depth + 1));

                    tokens.Add(new EnclosureMark(sourceText, enclosure.Close, depth));
                    currentIndex = enclosure.Close + 1;

                    while (currentIndex < endIndex && sourceText[currentIndex] == ' ')
                        currentIndex++;

                    continue;
                }

                if (unmatchedStartIndex == -1)
                    unmatchedStartIndex = currentIndex;

                // The next place a match could start: the next enclosure, else the clause's end.
                int nextBoundary = NextOpening(enclosures, currentIndex, clauseEndIndex);

                // Nothing left in the candidate set can match before that boundary (see
                // anyCandidateAllowsPartialClause), so skip to it in one step rather than ratcheting a word at a
                // time. Purely an optimization: that span is unmatched either way and gets flushed as the very same
                // single UnmatchedString.
                bool skipToBoundary = requireWholeClauses
                    && !anyCandidateAllowsPartialClause
                    && !atSegmentStart
                    && currentIndex < nextBoundary;

                if (skipToBoundary)
                {
                    currentIndex = nextBoundary;
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

                // ...nor into an enclosure, which is either matched or tokenized from its opening.
                if (currentIndex > nextBoundary)
                    currentIndex = nextBoundary;
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

            return new(graphs, graphs.Any(x => x.AllowsPartialClauseMatch));
        });

    /// <summary>
    /// The scope ends a type may be matched against at <paramref name="currentIndex"/>, in the order they should be
    /// tried. A type not held to the whole-clause rule gets the full scope and nothing else (it may end partway,
    /// which <see cref="RegexGraph.TryMatch"/> decides on its own). A type held to it gets each boundary of the
    /// clause - the edges of each enclosure in it (see <see cref="Enclosures"/>), then the clause's end - and only if
    /// it declares a clause break of its own, each later clause's end in turn: shortest first, so it settles on the
    /// least text that satisfies it rather than the most.
    /// </summary>
    static IEnumerable<int> GetCandidateEnds(
        string sourceText, string matchText, Enclosures enclosures, int currentIndex, int endIndex, int clauseEndIndex, bool mustConsumeClause, bool spansClauses)
    {
        if (!mustConsumeClause)
        {
            yield return endIndex;
            yield break;
        }

        // Before an enclosure (less the space separating it) and just after it, for each one ahead in this clause.
        var enclosureEdges = enclosures.Outermost
            .Where(x => x.Open >= currentIndex && x.Close < clauseEndIndex)
            .SelectMany(x => new[] { TrimEnd(sourceText, currentIndex, x.Open), x.Close + 1 });

        foreach (var edge in enclosureEdges.Where(x => x > currentIndex && x < clauseEndIndex).Distinct().Order())
            yield return edge;

        yield return clauseEndIndex;

        if (!spansClauses)
            yield break;

        int candidateEnd = clauseEndIndex;

        // Each step moves past the period just offered and out to the next boundary, so the sequence
        // strictly increases and terminates at endIndex.
        while (candidateEnd < endIndex)
        {
            candidateEnd = FindClauseEnd(matchText, candidateEnd + 1, endIndex);
            yield return candidateEnd;
        }
    }

    /// <summary><paramref name="index"/> moved back over the spaces before it, no further than <paramref name="floor"/>.</summary>
    static int TrimEnd(string sourceText, int floor, int index)
    {
        while (index > floor && sourceText[index - 1] == ' ')
            index--;

        return index;
    }

    /// <summary>
    /// Whether a match may start at <paramref name="index"/> partway into a clause: at an enclosure's opening, or at
    /// the first text after one closes (see <see cref="GetCandidateEnds"/>, which offers the same edges as ends).
    /// </summary>
    static bool IsSegmentStart(string sourceText, Enclosures enclosures, int index) =>
        enclosures.Outermost.Any(x => x.Open == index || (x.Close < index && TrimEnd(sourceText, x.Close + 1, index) == x.Close + 1));

    /// <summary>Where the next enclosure after <paramref name="index"/> opens, if before <paramref name="clauseEndIndex"/> - else that.</summary>
    static int NextOpening(Enclosures enclosures, int index, int clauseEndIndex) =>
        enclosures.Outermost.Where(x => x.Open > index && x.Open < clauseEndIndex).Select(x => x.Open).DefaultIfEmpty(clauseEndIndex).First();

    /// <summary>
    /// The exclusive end of the clause starting at <paramref name="fromIndex"/>: the next period at or after it, or
    /// <paramref name="endIndex"/> if the scope runs out first. Read from the scope's match text (see
    /// <see cref="Enclosures.MatchText"/>), where only a period outside every enclosure is one. The period itself is
    /// never part of the clause - a whole-clause match ends immediately before it, leaving the period to the ordinary
    /// ratchet on a later pass, which emits it as a <see cref="ClauseBreak"/>.
    /// </summary>
    static int FindClauseEnd(string matchText, int fromIndex, int endIndex)
    {
        if (fromIndex >= endIndex)
            return endIndex;

        int periodIndex = matchText.IndexOf(ClauseBreak.Period, fromIndex);

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
