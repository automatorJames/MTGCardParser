namespace Glyphotype.RegexGeneration.Matching;

/// <summary>One group's capture within a match: the group's name relative to the plan it was captured in, and where in the text.</summary>
/// <param name="Pattern">For a group capturing an authored pattern's text: the pattern; else null.</param>
public readonly record struct PlanCapture(string Name, int Index, int Length, string Pattern = null);

/// <summary>What one match of a plan came to: where it ends, and what it captured (see <see cref="Captures"/>).</summary>
public sealed class PlanMatch
{
    public int End { get; }

    /// <summary>The captures, as a tree to be flattened once a match is kept: a match of a sequence shares its parts' captures rather than copying them.</summary>
    readonly object _captures;

    internal PlanMatch(int end, object captures)
    {
        End = end;
        _captures = captures;
    }

    static readonly PlanCapture[] _none = [];

    internal static PlanMatch Empty(int at) => new(at, _none);

    /// <summary>A match of a sequence: <paramref name="first"/> then <paramref name="second"/>.</summary>
    internal static PlanMatch Join(PlanMatch first, PlanMatch second) =>
        new(second.End, new Joined(first._captures, second._captures));

    /// <summary>A match of a called group: the group's own capture, then what its plan captured, under its name.</summary>
    internal static PlanMatch Called(string name, int index, PlanMatch inner) =>
        new(inner.End, new Nested(name, index, inner.End - index, inner._captures));

    sealed record Joined(object First, object Second);
    sealed record Nested(string Name, int Index, int Length, object Inner);

    /// <summary>Every capture of this match, in the order the regex engine would have made them, with names relative to the plan's root.</summary>
    public List<PlanCapture> Captures()
    {
        var captures = new List<PlanCapture>();
        Flatten(_captures, null, captures);
        return captures;
    }

    static void Flatten(object captures, string prefix, List<PlanCapture> into)
    {
        switch (captures)
        {
            case PlanCapture[] leaf:
                foreach (var capture in leaf)
                    into.Add(prefix is null || capture.Pattern is not null ? capture : capture with { Name = prefix + capture.Name });
                break;

            case Joined joined:
                Flatten(joined.First, prefix, into);
                Flatten(joined.Second, prefix, into);
                break;

            case Nested nested:
                into.Add(new(prefix + nested.Name, nested.Index, nested.Length));
                Flatten(nested.Inner, prefix + nested.Name + "_", into);
                break;
        }
    }
}

/// <summary>
/// Runs a <see cref="MatchPlan"/> against a window of text, finding its matches in the order the flattened regex's
/// engine would: a leaf's by the engine itself, and after the first, each next by running the leaf again with the ends
/// already tried ruled out - which is the engine's own next alternative, since nothing after a leaf depends on more
/// than where it ended; a sequence's by backtracking through its items'; an alternation's in order; a quantified
/// call's greedily. A call's matches at a place are found once per window and kept (see <see cref="Session"/>), for
/// every plan calling it there.
/// </summary>
public static class Matcher
{
    /// <summary>
    /// The matches of <paramref name="plan"/> starting at <paramref name="index"/> of <paramref name="text"/>, in priority order,
    /// matched within the window [<paramref name="windowStart"/>, <paramref name="windowEnd"/>) - what the engine sees of the
    /// text, as when a regex is run against a bounded window: anchors and lookarounds stop at its edges.
    /// </summary>
    public static IEnumerable<PlanMatch> Matches(MatchPlan plan, string text, int index, int windowStart, int windowEnd) =>
        Session.For(text, windowStart, windowEnd).Matches(plan, index);

    /// <summary>The first match of <paramref name="plan"/> at <paramref name="index"/> - or, when <paramref name="mustFillWindow"/>, the first ending at the window's end - or null.</summary>
    public static PlanMatch First(MatchPlan plan, string text, int index, int windowStart, int windowEnd, bool mustFillWindow)
    {
        foreach (var match in Matches(plan, text, index, windowStart, windowEnd))
            if (!mustFillWindow || match.End == windowEnd)
                return match;

        return null;
    }

    /// <summary>Whether <paramref name="pattern"/> can match in only one way at a place: no alternation, quantifier or group with a prefix outside a character class.</summary>
    internal static bool IsDeterministic(string pattern)
    {
        var inClass = false;

        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];

            if (c == '\\')
            {
                i++;
                continue;
            }

            if (inClass)
            {
                inClass = c != ']';
                continue;
            }

            if (c == '[')
                inClass = true;
            else if (c is '|' or '?' or '*' or '+' or '{')
                return false;
        }

        return true;
    }

    /// <summary>
    /// One window of text being matched - a line, or the part of it a dynamic slot captured - and what matching found in it so
    /// far: the matches of each called plan at each place, kept for every plan that calls it there (the same nested glyph is
    /// tried at the same place by every type that nests it), and the regexes that rule out a leaf's tried ends.
    /// </summary>
    sealed class Session
    {
        public string Text { get; }
        public int WindowStart { get; }
        public int WindowEnd { get; }

        /// <summary>The window's text, which leaves are run against - so the engine sees nothing outside it, as it wouldn't of a bounded window.</summary>
        readonly string _window;

        readonly Dictionary<(MatchPlan, int), Memo> _memos = [];
        readonly Dictionary<(string, string), Regex> _exclusions = [];

        Session(string text, int windowStart, int windowEnd)
        {
            Text = text;
            WindowStart = windowStart;
            WindowEnd = windowEnd;
            _window = windowStart == 0 && windowEnd == text.Length ? text : text.Substring(windowStart, windowEnd - windowStart);
        }

        [ThreadStatic]
        static List<Session> _recent;

        const int _recentCapacity = 8;

        /// <summary>The session for a window - the one used lately for it, if any, so what it found is kept across the types tried there.</summary>
        public static Session For(string text, int windowStart, int windowEnd)
        {
            _recent ??= new(_recentCapacity);

            for (var i = _recent.Count - 1; i >= 0; i--)
            {
                var session = _recent[i];

                if (ReferenceEquals(session.Text, text) && session.WindowStart == windowStart && session.WindowEnd == windowEnd)
                    return session;
            }

            var created = new Session(text, windowStart, windowEnd);

            if (_recent.Count == _recentCapacity)
                _recent.RemoveAt(0);

            _recent.Add(created);
            return created;
        }

        public IEnumerable<PlanMatch> Matches(MatchPlan plan, int index) =>
            Matches(plan.Root, index);

        IEnumerable<PlanMatch> Matches(PlanNode node, int index) =>
            node switch
            {
                PlanNode.Leaf leaf => LeafMatches(leaf, index),
                PlanNode.Sequence sequence => SequenceMatches(sequence.Items, 0, index),
                PlanNode.Alternation alternation => alternation.Alternatives.SelectMany(x => Matches(x, index)),
                PlanNode.Call call => CallMatches(call, index),
                _ => throw new InvalidOperationException($"Unknown plan node {node.GetType().Name}"),
            };

        IEnumerable<PlanMatch> SequenceMatches(PlanNode[] items, int item, int index)
        {
            if (item == items.Length)
            {
                yield return PlanMatch.Empty(index);
                yield break;
            }

            foreach (var first in Matches(items[item], index))
                foreach (var rest in SequenceMatches(items, item + 1, first.End))
                    yield return PlanMatch.Join(first, rest);
        }

        IEnumerable<PlanMatch> CallMatches(PlanNode.Call call, int index)
        {
            switch (call.Quantifier)
            {
                case null:
                    foreach (var inner in Memoized(call.Target, index))
                        yield return PlanMatch.Called(call.Name, index, inner);
                    break;

                case Quantifier.Optional:
                    foreach (var inner in Memoized(call.Target, index))
                        yield return PlanMatch.Called(call.Name, index, inner);

                    yield return PlanMatch.Empty(index);
                    break;

                case Quantifier.AnyNumber:
                    foreach (var match in Repeated(call, index))
                        yield return match;
                    break;
            }
        }

        /// <summary>A repeated group's matches, greedily: as many repetitions as can be, fewer only when what follows fails - and none at all last.</summary>
        IEnumerable<PlanMatch> Repeated(PlanNode.Call call, int index)
        {
            foreach (var inner in Memoized(call.Target, index))
            {
                // A repetition matching nothing would repeat forever; the engine doesn't take one either.
                if (inner.End == index)
                    continue;

                var once = PlanMatch.Called(call.Name, index, inner);

                foreach (var rest in Repeated(call, inner.End))
                    yield return PlanMatch.Join(once, rest);
            }

            yield return PlanMatch.Empty(index);
        }

        /// <summary>The matches of <paramref name="plan"/> at <paramref name="index"/>, found once and replayed to every caller.</summary>
        IEnumerable<PlanMatch> Memoized(MatchPlan plan, int index)
        {
            if (!_memos.TryGetValue((plan, index), out var memo))
                _memos[(plan, index)] = memo = new(Matches(plan.Root, index).GetEnumerator());

            return memo.Replay();
        }

        sealed class Memo(IEnumerator<PlanMatch> source)
        {
            readonly List<PlanMatch> _known = [];
            bool _exhausted;

            public IEnumerable<PlanMatch> Replay()
            {
                var i = 0;

                while (true)
                {
                    if (i < _known.Count)
                        yield return _known[i++];
                    else if (_exhausted)
                        yield break;
                    else if (source.MoveNext())
                        _known.Add(source.Current);
                    else
                    {
                        _exhausted = true;
                        yield break;
                    }
                }
            }
        }

        IEnumerable<PlanMatch> LeafMatches(PlanNode.Leaf leaf, int index)
        {
            var at = index - WindowStart;
            var match = leaf.Regex.Match(_window, at);

            if (!match.Success)
                yield break;

            yield return Captured(leaf, match);

            if (leaf.IsDeterministic)
                yield break;

            // Each next match: the engine's next alternative, with every end already tried ruled out.
            var excluded = new List<int> { match.Index + match.Length };

            while (true)
            {
                match = Exclusion(leaf, excluded).Match(_window, at);

                if (!match.Success)
                    yield break;

                yield return Captured(leaf, match);
                excluded.Add(match.Index + match.Length);
            }
        }

        /// <summary><paramref name="leaf"/>'s regex, refusing a match ending at any of <paramref name="ends"/> (positions in the window).</summary>
        Regex Exclusion(PlanNode.Leaf leaf, List<int> ends)
        {
            var key = (leaf.Pattern, string.Join(",", ends));

            if (!_exclusions.TryGetValue(key, out var regex))
                _exclusions[key] = regex = new(@"\G(?:" + leaf.Pattern + ")(?!" + string.Join("|", ends.Select(x => @"(?<=\A[\s\S]{" + x + "})")) + ")", RegexOptions.ExplicitCapture);

            return regex;
        }

        PlanMatch Captured(PlanNode.Leaf leaf, Match match)
        {
            var captures = new List<PlanCapture>();

            for (var i = 0; i < leaf.GroupNumbers.Length; i++)
            {
                var group = match.Groups[leaf.GroupNumbers[i]];

                if (!group.Success)
                    continue;

                foreach (System.Text.RegularExpressions.Capture capture in group.Captures)
                    captures.Add(new(leaf.GroupNames[i], capture.Index + WindowStart, capture.Length, leaf.Patterns[i]));
            }

            return new(match.Index + match.Length + WindowStart, captures.ToArray());
        }
    }
}
