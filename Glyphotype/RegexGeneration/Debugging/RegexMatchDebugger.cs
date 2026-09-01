namespace Glyphotype.RegexGeneration.Debugging;

/// <summary>
/// Tests a <see cref="RegexGraph"/> against a text segment by building successively longer "stemmed
/// permutations" of it — a prefix of the graph's meaningful units with every still-open group closed off so
/// each stem compiles as a valid regex — until a unit fails to match, localizing exactly where the graph
/// stopped agreeing with the text. A meaningful unit is a literal text line, a joiner line, or a whole
/// named group; a failing sequential named group is decomposed into its own inner units and walked the same
/// way, while alternation-style groups (enums, pipe-joined one-ofs) stay atomic — their whole-group test
/// already is the meaningful question, and cutting a stem mid-alternation would change its semantics
/// (<c>(?&lt;X&gt;a</c> + <c>)</c> matches only <c>a</c> where the full group matched <c>a|b</c>). A dynamic
/// group is tested the way runtime resolves it: by substituting each registered glyph type's own compiled
/// graph in place of its greedy placeholder pattern — the placeholder itself (<c>[^.]+</c>) matches almost
/// anything, so testing it directly would pass text no actual glyph resolves.
/// <para>
/// Every stem test is one independent, start-anchored <see cref="Regex"/> match against the full segment,
/// so normal backtracking applies within each stem; the walk only decides which stems are worth testing.
/// Stems cut inside an optional/any-number group inherit that group's quantifier on the synthesized close,
/// which means such a group can never register as the failure point — correctly mirroring runtime, where an
/// optional group is skipped rather than failing the match.
/// </para>
/// </summary>
public static class RegexMatchDebugger
{
    /// <summary>Per-stem match timeout, so one pathological backtracking case can't hang the whole analysis pass.</summary>
    static readonly TimeSpan _matchTimeout = TimeSpan.FromSeconds(1);

    /// <summary>Analyzes one graph against <paramref name="textSegment"/> (which must already be trimmed to word boundaries).</summary>
    public static RegexDebugResult Analyze(RegexGraph graph, string textSegment) =>
        new StemWalk(graph, textSegment).Run();

    class StemWalk
    {
        readonly RegexGraph _graph;
        readonly string _text;
        readonly List<RegexBrick> _bricks;

        /// <summary>The committed (known-matching) regex text so far — brick text with spaces still escaped.</summary>
        readonly StringBuilder _stem = new();

        /// <summary>Groups whose open bookend is committed but whose close is not yet, outermost first.</summary>
        readonly List<NamedGroupNode> _openGroups = [];

        /// <summary>Exclusive end index of the committed prefix of <see cref="_bricks"/>.</summary>
        int _committedBrickEnd;

        int _bestMatchLength;
        int _matchedUnits;

        RegexNode _failureNode;
        RegexBrick _failureBrick;

        public StemWalk(RegexGraph graph, string textSegment)
        {
            _graph = graph;
            _text = textSegment;
            _bricks = graph.BuiltRegex.Bricks;
        }

        public RegexDebugResult Run()
        {
            // Whole-graph fast path: when the complete regex already matches from the segment's start,
            // there's nothing to localize. Only trustworthy for a graph with no dynamic groups — a dynamic
            // group's own placeholder pattern ([^.]+) matches nearly anything, so the compiled regex
            // passing says nothing about whether any registered glyph actually resolves there. Graphs with
            // dynamic groups always take the full walk, whose substitution testing verifies that.
            bool hasDynamicNodes = _graph.NamedGroupFlatGraph.Values.Any(x => x is DynamicGlyphNode);

            if (!hasDynamicNodes && TestCandidate(_graph.BuiltRegex.MinifiedRegex, out int fullLength))
            {
                _bestMatchLength = fullLength;
                _committedBrickEnd = _bricks.Count;
                _matchedUnits = CountAllUnits();
                return BuildResult(isFullMatch: true);
            }

            var root = _graph.RootNode;
            bool completed = false;

            if (root.IsTransparentRoot)
            {
                if (IsDecomposable(root))
                    completed = WalkGroup(root, 0, _bricks.Count);
                else
                    SetFailure(root, _bricks.FirstOrDefault());
            }
            else
            {
                // A non-transparent root wraps everything in its own bookends; commit the open (zero-width)
                // and walk its inner range, exactly as any nested decomposable group is walked.
                int closeIdx = FindCloseIndex(root, 0);

                if (IsDecomposable(root) && closeIdx > 0)
                {
                    CommitBrickText(_bricks[0], 1);
                    _openGroups.Add(root);

                    if (completed = WalkGroup(root, 1, closeIdx))
                    {
                        _openGroups.RemoveAt(_openGroups.Count - 1);
                        CommitBrickText(_bricks[closeIdx], closeIdx + 1);
                    }
                }
                else
                {
                    SetFailure(root, _bricks.FirstOrDefault());
                }
            }

            // Every unit passed (with any dynamic groups verified via substitution) — a full match reached
            // by walking rather than by the fast path above.
            return BuildResult(isFullMatch: completed);
        }

        /// <summary>
        /// Walks the meaningful units strictly inside <paramref name="group"/>'s bookends (brick indexes
        /// [<paramref name="innerStart"/>, <paramref name="innerEnd"/>)), committing each passing unit into
        /// the stem. Returns false as soon as a unit fails (the failure has been recorded by then).
        /// </summary>
        bool WalkGroup(NamedGroupNode group, int innerStart, int innerEnd)
        {
            int i = innerStart;

            while (i < innerEnd)
            {
                var brick = _bricks[i];

                if (brick is RegexBrickGroupOpen && brick.Parent is NamedGroupNode child && child.ParentNode == group)
                {
                    int closeIdx = FindCloseIndex(child, i);

                    if (!TryCommitGroupUnit(child, i, closeIdx))
                        return false;

                    i = closeIdx + 1;
                    continue;
                }

                // Everything else at this level is a single-brick unit: a literal text line, or a joiner
                // owned by this group. (A nullable child's embedded joiners sit inside its own bookend
                // range, which the group-unit branch above consumes wholesale.)
                if (!TryCommitSingleBrickUnit(brick, i))
                    return false;

                i++;
            }

            return true;
        }

        /// <summary>Tests the stem extended by one literal/joiner brick; commits it on success, records the failure otherwise.</summary>
        bool TryCommitSingleBrickUnit(RegexBrick brick, int index)
        {
            if (TestCandidate(_stem.ToString() + brick.Regex + Closers(), out int length))
            {
                CommitBrickText(brick, index + 1);
                _matchedUnits++;
                _bestMatchLength = Math.Max(_bestMatchLength, length);
                return true;
            }

            SetFailure(brick.Parent, brick);
            return false;
        }

        /// <summary>
        /// Tests a whole child named group as one unit. On failure, a dynamic group is retried via glyph
        /// substitution, a sequential group is decomposed and walked internally, and everything else
        /// (enum, pipe-joined alternation, childless leaf) is recorded as the failure point itself.
        /// </summary>
        bool TryCommitGroupUnit(NamedGroupNode child, int openIdx, int closeIdx)
        {
            // A dynamic group is never tested via its own raw bricks — its placeholder pattern would pass
            // trivially — it goes straight to substitution testing.
            if (child is DynamicGlyphNode dynamicNode)
                return TryCommitDynamicUnit(dynamicNode, openIdx, closeIdx);

            // A group with a dynamic descendant is equally untrustworthy as a whole unit, so skip the
            // whole-group shortcut and decompose it directly whenever that's possible; the dynamic
            // descendant then gets its proper substitution test on the way down. (A non-decomposable
            // group containing one — e.g. a pipe-joined one-of with a dynamic alternative — falls through
            // to the lenient whole test below, the best remaining approximation.)
            bool mustDecompose = IsDecomposable(child) && ContainsDynamicNode(child);

            if (!mustDecompose)
            {
                var unitText = ConcatBrickText(openIdx, closeIdx + 1);

                if (TestCandidate(_stem.ToString() + unitText + Closers(), out int length))
                {
                    _stem.Append(unitText);
                    _committedBrickEnd = closeIdx + 1;
                    _matchedUnits += CountUnitsForWholeGroup(child);
                    _bestMatchLength = Math.Max(_bestMatchLength, length);
                    return true;
                }

                if (!IsDecomposable(child))
                {
                    SetFailure(child, _bricks[openIdx]);
                    return false;
                }
            }

            // Decompose: commit the open bookend (zero-width on its own) and walk the group's inner units,
            // with this group now contributing its close (plus quantifier) to every deeper stem's closers.
            CommitBrickText(_bricks[openIdx], openIdx + 1);
            _openGroups.Add(child);

            if (!WalkGroup(child, openIdx + 1, closeIdx))
                return false;

            // Every inner unit passed individually even though the whole group's own test failed — a
            // backtracking edge case. Close the group out and let the outer walk continue.
            _openGroups.RemoveAt(_openGroups.Count - 1);
            CommitBrickText(_bricks[closeIdx], closeIdx + 1);
            return true;
        }

        /// <summary>
        /// Tests a dynamic group the way runtime resolution works: for each registered tokenizer type
        /// (scoped by the property's <see cref="TypeFilterAttribute"/>, if any), substitute that type's
        /// whole compiled graph for the group's greedy placeholder content and test the resulting stem.
        /// Commits the substitution that matched the most text, so the walk can continue past the group
        /// with a concrete (rather than match-anything) resolution in place.
        /// </summary>
        bool TryCommitDynamicUnit(DynamicGlyphNode dynamicNode, int openIdx, int closeIdx)
        {
            var filterType = dynamicNode.Navigation.Prop?.GetCustomAttribute<TypeFilterAttribute>()?.Type ?? typeof(Glyph);

            var candidateTypes = filterType == typeof(Glyph)
                ? GlyphTypeRegistry.AppliedOrderTypes
                : GlyphTypeRegistry.AppliedOrderTypes.Where(x => x.IsAssignableTo(filterType)).ToList();

            var openText = _bricks[openIdx].Regex;
            var closeText = _bricks[closeIdx].Regex;

            string bestUnitText = null;
            int bestLength = -1;

            foreach (var type in candidateTypes)
            {
                if (!GlyphTypeRegistry.RegexGraphs.TryGetValue(type, out var substitutedGraph))
                    continue;

                var unitText = openText + substitutedGraph.BuiltRegex.MinifiedRegex + closeText;

                if (TestCandidate(_stem.ToString() + unitText + Closers(), out int length) && length > bestLength)
                {
                    bestUnitText = unitText;
                    bestLength = length;
                }
            }

            if (bestUnitText == null)
            {
                SetFailure(dynamicNode, _bricks[openIdx]);
                return false;
            }

            _stem.Append(bestUnitText);
            _committedBrickEnd = closeIdx + 1;
            _matchedUnits++;
            _bestMatchLength = Math.Max(_bestMatchLength, bestLength);
            return true;
        }

        /// <summary>
        /// Whether a failing group is worth walking internally: it must compose its children sequentially.
        /// Enums and dynamic groups are atomic by design (member alternatives are never split apart;
        /// dynamic groups have their own substitution handling), and pipe-joined groups (one-ofs) can't be
        /// stem-cut without changing what the alternation means.
        /// </summary>
        static bool IsDecomposable(NamedGroupNode group) =>
            group is not EnumNode
            && group is not DynamicGlyphNode
            && group.EffectiveChildJoiner != Joiner.Pipe
            && group.Children.Count > 0;

        /// <summary>Whether any named group anywhere in <paramref name="group"/>'s subtree is a dynamic group.</summary>
        static bool ContainsDynamicNode(NamedGroupNode group) =>
            group.NamedGroupChildren.Any(x => x is DynamicGlyphNode || ContainsDynamicNode(x));

        /// <summary>Appends one committed brick's text to the stem and advances the committed-prefix marker.</summary>
        void CommitBrickText(RegexBrick brick, int newCommittedEnd)
        {
            _stem.Append(brick.Regex);
            _committedBrickEnd = newCommittedEnd;
        }

        /// <summary>The synthesized tail that closes every currently-open group (innermost first), each with its own real quantifier.</summary>
        string Closers()
        {
            var sb = new StringBuilder();

            for (int i = _openGroups.Count - 1; i >= 0; i--)
                sb.Append(')').Append(_openGroups[i].Quantifier is { } quantifier ? quantifier.GetDescription() : "");

            return sb.ToString();
        }

        string ConcatBrickText(int start, int endExclusive)
        {
            var sb = new StringBuilder();

            for (int i = start; i < endExclusive; i++)
                sb.Append(_bricks[i].Regex);

            return sb.ToString();
        }

        /// <summary>Index of <paramref name="group"/>'s own close bookend at/after <paramref name="fromIdx"/>. Each node emits exactly one open/close pair, so parent identity alone finds it.</summary>
        int FindCloseIndex(NamedGroupNode group, int fromIdx)
        {
            for (int i = fromIdx; i < _bricks.Count; i++)
                if (_bricks[i] is RegexBrickGroupClose && ReferenceEquals(_bricks[i].Parent, group))
                    return i;

            return -1;
        }

        /// <summary>
        /// One start-anchored test of a candidate stem against the segment. The candidate arrives as brick
        /// text (spaces escaped, same as the graph writes them) and is unescaped exactly the way
        /// <see cref="BuiltRegex"/> unescapes the full pattern before compiling. Matches the same
        /// <see cref="RegexOptions.ExplicitCapture"/> semantics the runtime regex compiles with; a
        /// zero-length match still counts as a pass (an all-optional stem legitimately matches nothing).
        /// </summary>
        bool TestCandidate(string candidatePattern, out int matchLength)
        {
            matchLength = 0;
            var pattern = candidatePattern.Replace(BuiltRegex.EscapedSpace, " ");

            try
            {
                var regex = new Regex(pattern, RegexOptions.ExplicitCapture, _matchTimeout);
                var match = regex.Match(_text);

                if (!match.Success || match.Index != 0)
                    return false;

                matchLength = match.Length;
                return true;
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        void SetFailure(RegexNode node, RegexBrick brick)
        {
            _failureNode = node;
            _failureBrick = brick;
        }

        // --- Unit counting (same decomposition rules as the walk itself) ---

        int CountAllUnits()
        {
            var root = _graph.RootNode;

            if (!IsDecomposable(root))
                return 1;

            var (innerStart, innerEnd) = root.IsTransparentRoot
                ? (0, _bricks.Count)
                : (1, FindCloseIndex(root, 0));

            return Math.Max(1, CountUnitsInRange(root, innerStart, innerEnd));
        }

        /// <summary>How many units a wholly-passed child group contributes: its recursive inner count when it would have been decomposed on failure, 1 otherwise.</summary>
        int CountUnitsForWholeGroup(NamedGroupNode group)
        {
            if (!IsDecomposable(group))
                return 1;

            int openIdx = _bricks.FindIndex(x => x is RegexBrickGroupOpen && ReferenceEquals(x.Parent, group));
            int closeIdx = FindCloseIndex(group, openIdx);

            return Math.Max(1, CountUnitsInRange(group, openIdx + 1, closeIdx));
        }

        int CountUnitsInRange(NamedGroupNode group, int innerStart, int innerEnd)
        {
            int count = 0;
            int i = innerStart;

            while (i >= 0 && i < innerEnd)
            {
                var brick = _bricks[i];

                if (brick is RegexBrickGroupOpen && brick.Parent is NamedGroupNode child && child.ParentNode == group)
                {
                    count += CountUnitsForWholeGroup(child);
                    i = FindCloseIndex(child, i) + 1;
                    continue;
                }

                count++;
                i++;
            }

            return count;
        }

        // --- Result assembly ---

        RegexDebugResult BuildResult(bool isFullMatch)
        {
            var stemBricks = _bricks.Take(_committedBrickEnd).ToList();

            // Close every still-open group with its *real* close bookend brick, innermost first, so the
            // displayed stem renders with exactly the metadata (comments, quantifier, coloring identity)
            // the full graph's own rendering gives that same line.
            for (int i = _openGroups.Count - 1; i >= 0; i--)
            {
                int closeIdx = FindCloseIndex(_openGroups[i], 0);

                if (closeIdx >= 0)
                    stemBricks.Add(_bricks[closeIdx]);
            }

            return new RegexDebugResult
            {
                GlyphType = _graph.RootGlyphType,
                Graph = _graph,
                TextSegment = _text,
                IsFullMatch = isFullMatch,
                MatchedCharCount = _bestMatchLength,
                MatchedWordCount = CountCoveredWords(_text, _bestMatchLength),
                TotalWordCount = _text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                MatchedUnitCount = _matchedUnits,
                TotalUnitCount = CountAllUnits(),
                FirstFailureDisplay = isFullMatch ? "" : GetFailureDisplay(),
                FirstFailureFullyQualifiedName = FailedAsGroupUnit ? ((NamedGroupNode)_failureNode).FullyQualifiedName : null,
                FirstFailureBrick = isFullMatch ? null : _failureBrick,
                MaxMatchStemBricks = stemBricks,
                MaxMatchStemRegex = (_stem.ToString() + Closers()).Replace(BuiltRegex.EscapedSpace, " "),
            };
        }

        /// <summary>
        /// Whether the first failure was a whole named group tested as one unit (its open bookend is the
        /// failure brick) — as opposed to a single literal/joiner brick, whose *owner* may also be a named
        /// group (a joiner's Parent is the group it separates children of) without the group itself being
        /// what failed.
        /// </summary>
        bool FailedAsGroupUnit =>
            _failureBrick is RegexBrickGroupOpen && _failureNode is NamedGroupNode;

        string GetFailureDisplay() =>
            FailedAsGroupUnit
                ? ((NamedGroupNode)_failureNode).FullyQualifiedName
                : _failureBrick?.Regex ?? _failureNode?.ToString() ?? "";

        /// <summary>How many of <paramref name="text"/>'s space-separated words fall entirely within the first <paramref name="matchLength"/> characters.</summary>
        static int CountCoveredWords(string text, int matchLength)
        {
            int covered = 0;
            int wordStart = -1;

            for (int i = 0; i <= text.Length; i++)
            {
                bool isSpace = i == text.Length || text[i] == ' ';

                if (!isSpace && wordStart < 0)
                    wordStart = i;

                if (isSpace && wordStart >= 0)
                {
                    if (i <= matchLength)
                        covered++;

                    wordStart = -1;
                }
            }

            return covered;
        }
    }
}
