using System.Collections.Concurrent;

namespace Glyphotype.RegexGeneration.Matching;

/// <summary>
/// A glyph type's pattern as the matcher runs it (see <see cref="Matcher"/>): the type's flattened regex
/// (<see cref="BuiltRegex.MinifiedRegex"/>) cut into <see cref="Leaf"/>s - runs of it compiled and run as they are -
/// and <see cref="Call"/>s - nested glyph groups large enough to be matched by a plan of their own, one plan per
/// distinct group content, shared by every type that nests it. Matching the plan finds the very same matches, in the
/// very same order, as running the flattened regex would: a leaf is matched by the regex engine, its alternatives
/// tried in the engine's own order (see <see cref="Matcher"/>), and the plan's own structure - sequence, alternation,
/// a group's quantifier - follows the engine's backtracking order too. What's gained is that a nested glyph's text is
/// held and compiled once however many types nest it, and matched once per place in a line however many types try it there.
/// </summary>
public abstract class PlanNode
{
    /// <summary>A run of the flattened regex, run as a regex of its own.</summary>
    public sealed class Leaf : PlanNode
    {
        /// <summary>The run's text, with its group names relative to the plan's root (see <see cref="MatchPlan.Relativize"/>).</summary>
        public string Pattern { get; }

        /// <summary><see cref="Pattern"/> anchored to where it's run from.</summary>
        public Regex Regex { get; }

        /// <summary>The names of the groups the run captures - a property's, relative to the plan's root - and, for an authored pattern's group (see <see cref="PatternGroupPrefix"/>), null.</summary>
        internal string[] GroupNames { get; }

        /// <summary>The group number of each name in <see cref="GroupNames"/>.</summary>
        internal int[] GroupNumbers { get; }

        /// <summary>The authored pattern each pattern group captures, by the group's position in <see cref="GroupNames"/> - null for a property's group.</summary>
        internal string[] Patterns { get; }

        /// <summary>
        /// Whether the run can match only one way at a place: no alternation, quantifier or lookaround outside a character
        /// class - so the first match is the only one, and nothing need be tried after it fails what follows.
        /// </summary>
        internal bool IsDeterministic { get; }

        internal Leaf(string pattern, IReadOnlyList<string> patterns)
        {
            Pattern = pattern;
            Regex = BuiltRegex.Compile(@"\G(?:" + pattern + ")");
            IsDeterministic = Matcher.IsDeterministic(pattern);

            var names = Regex.GetGroupNames().Where(x => x != "0").ToList();
            GroupNames = names.Select(x => x.StartsWith(PatternGroupPrefix, StringComparison.Ordinal) ? null : x).ToArray();
            GroupNumbers = names.Select(Regex.GroupNumberFromName).ToArray();
            Patterns = names.Select(x => x.StartsWith(PatternGroupPrefix, StringComparison.Ordinal) ? patterns[int.Parse(x.AsSpan(PatternGroupPrefix.Length))] : null).ToArray();
        }
    }

    /// <summary>Items matched one after another.</summary>
    public sealed class Sequence(PlanNode[] items) : PlanNode
    {
        public PlanNode[] Items { get; } = items;
    }

    /// <summary>Alternatives tried in order, as a regex alternation is.</summary>
    public sealed class Alternation(PlanNode[] alternatives) : PlanNode
    {
        public PlanNode[] Alternatives { get; } = alternatives;
    }

    /// <summary>A nested glyph's group, matched by its own shared plan, and captured under <see cref="Name"/>.</summary>
    public sealed class Call(string name, Quantifier? quantifier, MatchPlan target) : PlanNode
    {
        /// <summary>The group's name, relative to the plan's root.</summary>
        public string Name { get; } = name;

        /// <summary>The group's own quantifier, if any: an optional nested glyph, or a repeated list item.</summary>
        public Quantifier? Quantifier { get; } = quantifier;

        /// <summary>The plan of the group's content.</summary>
        public MatchPlan Target { get; } = target;
    }

    /// <summary>The prefix of a group capturing what an authored pattern matched (see <see cref="Nodes.TextNode.AuthoredPattern"/>), followed by the pattern's number within its leaf.</summary>
    internal const string PatternGroupPrefix = "__p";
}

/// <summary>A plan for a whole type's pattern, or for a nested group's content (see <see cref="PlanNode"/>).</summary>
public sealed class MatchPlan
{
    /// <summary>What the plan matches.</summary>
    public PlanNode Root { get; private set; }

    /// <summary>The text the plan was built from, relative to its root: what tells one plan from another (see <see cref="Shared"/>).</summary>
    public string Key { get; }

    /// <summary>How many leaves and calls the plan holds - a measure of what it costs to run beyond the regex engine's own work.</summary>
    public int Size { get; private set; }

    MatchPlan(string key) => Key = key;

    /// <summary>
    /// The smallest nested glyph group, in characters of flattened regex, matched by a plan of its own rather than as part
    /// of the run of regex around it. Below it, the regex engine's own matching of the inlined text is cheaper than a call.
    /// </summary>
    public static int CallThreshold { get; internal set; } = 2_000;

    /// <summary>
    /// The plan of a type's pattern, from the bricks its graph produces (see <see cref="BuiltRegex.Bricks"/>) - built while they're
    /// in hand, since building them again costs more than anything here.
    /// </summary>
    internal static MatchPlan Build(GlyphNode root, List<RegexBrick> bricks)
    {
        var builder = new Builder(bricks);
        var prefix = root.FullyQualifiedName + "_";
        var key = builder.RelativeText(0, bricks.Count, prefix);
        return Shared(key, _ => builder.Parse(0, bricks.Count, root, prefix));
    }

    /// <summary>The plan keyed by <paramref name="text"/> - under the call threshold in force, which decides the plan's shape as much as the text does.</summary>
    static MatchPlan Shared(string text, Func<MatchPlan, PlanNode> build) =>
        SharedByKey($"{CallThreshold}:{text}", build);

    /// <summary>The plan keyed <paramref name="key"/>, built by <paramref name="build"/> if no live one has that key - so a nested glyph's content is one plan however many types nest it, and across the generations a definition's grammars are built in.</summary>
    static MatchPlan SharedByKey(string key, Func<MatchPlan, PlanNode> build)
    {
        lock (_shared)
        {
            if (_shared.TryGetValue(key, out var reference) && reference.TryGetTarget(out var plan) && plan.Root is not null)
                return plan;

            if (++_sharedSinceSweep > 256)
            {
                foreach (var dead in _shared.Where(x => !x.Value.TryGetTarget(out _)).Select(x => x.Key).ToList())
                    _shared.Remove(dead);

                _sharedSinceSweep = 0;
            }

            plan = new MatchPlan(key);
            plan.Root = build(plan);
            plan.Size = Count(plan.Root);
            _shared[key] = new(plan);
            return plan;
        }
    }

    static readonly Dictionary<string, WeakReference<MatchPlan>> _shared = [];
    static int _sharedSinceSweep;

    static int Count(PlanNode node) => node switch
    {
        PlanNode.Sequence sequence => sequence.Items.Sum(Count),
        PlanNode.Alternation alternation => alternation.Alternatives.Sum(Count),
        _ => 1,
    };

    /// <summary><paramref name="fullyQualifiedName"/> without <paramref name="prefix"/>, which every name under a plan's root starts with.</summary>
    internal static string Relativize(string fullyQualifiedName, string prefix) =>
        fullyQualifiedName.StartsWith(prefix, StringComparison.Ordinal)
            ? fullyQualifiedName[prefix.Length..]
            : throw new InvalidOperationException($"Group '{fullyQualifiedName}' isn't under '{prefix}'");

    /// <summary>Cuts a brick sequence into a plan (see <see cref="PlanNode"/>).</summary>
    sealed class Builder
    {
        readonly List<RegexBrick> _bricks;

        /// <summary>The index of each group-open brick's close, and each close's open.</summary>
        readonly int[] _partner;

        /// <summary>The regex length of each named glyph group, by the index of its open brick - what decides whether it's called.</summary>
        readonly int[] _groupLength;

        public Builder(List<RegexBrick> bricks)
        {
            _bricks = bricks;
            _partner = new int[bricks.Count];
            _groupLength = new int[bricks.Count];
            var open = new Stack<(int Index, int Length)>();
            var lengths = new int[bricks.Count + 1];

            for (var i = 0; i < bricks.Count; i++)
                lengths[i + 1] = lengths[i] + bricks[i].Regex.Length;

            for (var i = 0; i < bricks.Count; i++)
            {
                if (bricks[i] is RegexBrickGroupOpen)
                    open.Push((i, 0));
                else if (bricks[i] is RegexBrickGroupClose)
                {
                    var (openIndex, _) = open.Pop();
                    _partner[openIndex] = i;
                    _partner[i] = openIndex;
                    _groupLength[openIndex] = lengths[i + 1] - lengths[openIndex];
                }
            }

            if (open.Count > 0)
                throw new InvalidOperationException("A group is opened and never closed");
        }

        /// <summary>Whether the group opening at <paramref name="index"/> is a nested glyph's, large enough to be matched by its own plan (see <see cref="CallThreshold"/>).</summary>
        bool IsCall(int index) =>
            _bricks[index] is RegexBrickGroupOpen { Parent: GlyphNode node }
            && node is not DynamicGlyphNode
            && node.Navigation.IsGlyphType
            && node.ParentNode is not null
            && _groupLength[index] >= CallThreshold;

        /// <summary>The text of the bricks in [<paramref name="start"/>, <paramref name="end"/>), with group names relative to <paramref name="prefix"/> and authored patterns in groups of their own - the text a leaf of it would have.</summary>
        public string RelativeText(int start, int end, string prefix)
        {
            var text = new StringBuilder();
            var patterns = 0;

            for (var i = start; i < end; i++)
                AppendBrick(text, _bricks[i], prefix, ref patterns);

            return text.ToString().Replace(BuiltRegex.EscapedSpace, " ");
        }

        static void AppendBrick(StringBuilder text, RegexBrick brick, string prefix, ref int patterns)
        {
            if (brick is RegexBrickGroupOpen open)
                text.Append("(?<").Append(Relativize(open.GroupName, prefix)).Append('>');
            else if (brick.Parent is TextNode { AuthoredPattern: not null } node && brick.Regex == node.Text)
                text.Append(node.WithPatternGroup(PlanNode.PatternGroupPrefix + patterns++));
            else
                text.Append(brick.Regex);
        }

        /// <summary>
        /// The plan of the bricks in [<paramref name="start"/>, <paramref name="end"/>), the content of <paramref name="owner"/>'s group (or the whole
        /// pattern, for a root): alternatives split at the pipes the owner itself places between them, each a sequence of leaves and calls.
        /// </summary>
        public PlanNode Parse(int start, int end, NamedGroupNode owner, string prefix)
        {
            var alternatives = new List<PlanNode>();
            var items = new List<PlanNode>();
            var leaf = new StringBuilder();
            var leafPatterns = new List<string>();

            void FlushLeaf()
            {
                if (leaf.Length == 0)
                    return;

                items.Add(new PlanNode.Leaf(leaf.ToString().Replace(BuiltRegex.EscapedSpace, " "), [.. leafPatterns]));
                leaf.Clear();
                leafPatterns.Clear();
            }

            void FlushAlternative()
            {
                FlushLeaf();
                alternatives.Add(items.Count == 1 ? items[0] : new PlanNode.Sequence([.. items]));
                items = [];
            }

            void AppendLeafBrick(RegexBrick brick)
            {
                var patterns = leafPatterns.Count;

                if (brick.Parent is TextNode { AuthoredPattern: not null } node && brick.Regex == node.Text)
                    leafPatterns.Add(node.AuthoredPattern);

                AppendBrick(leaf, brick, prefix, ref patterns);
            }

            for (var i = start; i < end; i++)
            {
                var brick = _bricks[i];

                if (brick is RegexBrickJoiner { Joiner: Joiner.Pipe } && brick.Parent == owner)
                {
                    FlushAlternative();
                    continue;
                }

                // A plain group the owner puts around its alternatives (see GlyphOneOfNode): a level of its own, with the same owner.
                if (brick.Parent == owner && brick.GetType() == typeof(RegexBrick) && brick.Regex == "(")
                {
                    var close = FindPlainClose(i, owner);

                    if (ContainsCall(i + 1, close))
                    {
                        FlushLeaf();
                        items.Add(Parse(i + 1, close, owner, prefix));
                    }
                    else
                        for (var j = i; j <= close; j++)
                            AppendLeafBrick(_bricks[j]);

                    i = close;
                    continue;
                }

                if (brick is RegexBrickGroupOpen open && IsCall(i))
                {
                    FlushLeaf();
                    var close = _partner[i];
                    var node = (NamedGroupNode)brick.Parent;
                    var innerPrefix = open.GroupName + "_";
                    var key = RelativeText(i + 1, close, innerPrefix);
                    var target = Shared(key, _ => Parse(i + 1, close, node, innerPrefix));
                    items.Add(new PlanNode.Call(Relativize(open.GroupName, prefix), QuantifierOf((RegexBrickGroupClose)_bricks[close]), target));
                    i = close;
                    continue;
                }

                AppendLeafBrick(brick);
            }

            FlushAlternative();
            return alternatives.Count == 1 ? alternatives[0] : new PlanNode.Alternation([.. alternatives]);
        }

        /// <summary>Whether any group in [<paramref name="start"/>, <paramref name="end"/>) is a call - so the range can't be one leaf.</summary>
        bool ContainsCall(int start, int end)
        {
            for (var i = start; i < end; i++)
                if (_bricks[i] is RegexBrickGroupOpen && IsCall(i))
                    return true;

            return false;
        }

        /// <summary>The index of the plain ")" <paramref name="owner"/> closes the plain "(" at <paramref name="open"/> with.</summary>
        int FindPlainClose(int open, NamedGroupNode owner)
        {
            var depth = 0;

            for (var i = open; i < _bricks.Count; i++)
            {
                var brick = _bricks[i];

                if (brick.Parent != owner || brick.GetType() != typeof(RegexBrick))
                    continue;

                if (brick.Regex == "(")
                    depth++;
                else if (brick.Regex == ")" && --depth == 0)
                    return i;
            }

            throw new InvalidOperationException($"{owner.FullyQualifiedName} opens a plain group it never closes");
        }

        static Quantifier? QuantifierOf(RegexBrickGroupClose close) =>
            close.Regex switch
            {
                ")" => null,
                ")?" => Quantifier.Optional,
                ")*" => Quantifier.AnyNumber,
                var other => throw new InvalidOperationException($"Unexpected group close '{other}'"),
            };
    }
}
