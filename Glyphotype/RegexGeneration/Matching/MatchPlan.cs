using System.Security.Cryptography;

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
        /// <summary>The run's text, with its group names relative to the plan's root (see <see cref="MatchPlan"/>).</summary>
        public string Pattern { get; }

        readonly Lazy<Regex> _regex;

        /// <summary><see cref="Pattern"/> anchored to where it's run from - compiled on first use.</summary>
        public Regex Regex => _regex.Value;

        readonly IReadOnlyDictionary<string, string> _patterns;
        readonly Lazy<(string[] Names, int[] Numbers, string[] Patterns)> _groups;

        /// <summary>The names of the groups the run captures - a property's, relative to the plan's root - and, for an authored pattern's group (see <see cref="PatternGroupPrefix"/>), null.</summary>
        internal string[] GroupNames => _groups.Value.Names;

        /// <summary>The group number of each name in <see cref="GroupNames"/>.</summary>
        internal int[] GroupNumbers => _groups.Value.Numbers;

        /// <summary>The authored pattern each pattern group captures, by the group's position in <see cref="GroupNames"/> - null for a property's group.</summary>
        internal string[] Patterns => _groups.Value.Patterns;

        /// <summary>
        /// Whether the run can match only one way at a place: no alternation, quantifier or lookaround outside a character
        /// class - so the first match is the only one, and nothing need be tried after it fails what follows.
        /// </summary>
        internal bool IsDeterministic { get; }

        /// <param name="patterns">The authored pattern each pattern group (named with <see cref="PatternGroupPrefix"/>) captures, by group name.</param>
        internal Leaf(string pattern, IReadOnlyDictionary<string, string> patterns)
        {
            Pattern = pattern;
            _patterns = patterns;
            IsDeterministic = Matcher.IsDeterministic(pattern);
            _regex = new(() => BuiltRegex.Compile(@"\G(?:" + pattern + ")"));
            _groups = new(() =>
            {
                var names = Regex.GetGroupNames().Where(x => x != "0").ToList();

                return (
                    names.Select(x => IsPatternGroup(x) ? null : x).ToArray(),
                    names.Select(Regex.GroupNumberFromName).ToArray(),
                    names.Select(x => IsPatternGroup(x) ? _patterns[x] : null).ToArray());
            });
        }

        static bool IsPatternGroup(string name) => name.StartsWith(PatternGroupPrefix, StringComparison.Ordinal);
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

    /// <summary>
    /// The prefix of a group capturing what an authored pattern matched (see <see cref="Nodes.TextNode.AuthoredPattern"/>),
    /// followed by the name of the group the pattern sits in and the pattern's place among that group's children -
    /// unique within a plan, and the same wherever the group's content is nested.
    /// </summary>
    internal const string PatternGroupPrefix = "__p";
}

/// <summary>
/// A plan for a whole type's pattern, or for a nested group's content (see <see cref="PlanNode"/>). Group names within a
/// plan are relative to its root: for a type's plan, to the type's root group; for a nested group's, to that group - so
/// the same content nested in different types is one plan, and a match's captures are qualified by whoever made the match.
/// </summary>
public sealed class MatchPlan
{
    /// <summary>What the plan matches.</summary>
    public PlanNode Root { get; private set; }

    /// <summary>What tells one plan from another: a digest of its content, relative to its root (see <see cref="Shared"/>).</summary>
    public string Key { get; }

    /// <summary>How many leaves and calls the plan holds - a measure of what it costs to run beyond the regex engine's own work.</summary>
    public int Size { get; private set; }

    MatchPlan(string key) => Key = key;

    /// <summary>
    /// The smallest nested glyph group, in characters of flattened regex, matched by a plan of its own rather than as part
    /// of the run of regex around it. Below it, the regex engine's own matching of the inlined text is cheaper than a call.
    /// </summary>
    public static int CallThreshold { get; internal set; } = 2_000;

    /// <summary>The plan keyed by <paramref name="key"/>, built by <paramref name="build"/> if no live one has that key - so a nested glyph's content is one plan however many types nest it, and across the generations a definition's grammars are built in.</summary>
    static MatchPlan Shared(string key, Func<PlanNode> build)
    {
        key = $"{CallThreshold}:{key}";

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
            plan.Root = build();
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

    /// <summary>
    /// Builds a type's plan from the bricks its graph produces, as they're produced (see <see cref="BuiltRegex"/>): a
    /// <see cref="RegexCollector"/> that keeps no bricks, only what the plan and the pattern's length need of them. Each
    /// group opened starts a frame; a frame closed small is text inlined into the frame around it, as the flattened regex
    /// inlines it; one closed large becomes a call to its content's shared plan.
    /// </summary>
    internal sealed class Collector : RegexCollector
    {
        /// <summary>A group being collected: a glyph's named group, a plain group its owner puts around alternatives, or the root.</summary>
        sealed class Frame
        {
            /// <summary>The node whose group this is: the owner of the pipes and plain parentheses at this level.</summary>
            public NamedGroupNode Owner;

            /// <summary>For a glyph's group: its open brick, and so its name; null for the root or a plain group.</summary>
            public RegexBrickGroupOpen Open;

            /// <summary>The frame's whole regex text so far, bookends included and group names fully qualified - kept while the frame might still be inlined as text into the frame around it; null once it can't be.</summary>
            public StringBuilder Text = new();

            /// <summary>The frame's content as structure: text runs, calls, plain levels and the pipes between alternatives.</summary>
            public List<Piece> Pieces = [];

            /// <summary>The run of text at the frame's own level since the last call or pipe.</summary>
            public StringBuilder Leaf = new();

            /// <summary>The authored pattern each pattern group anywhere in the frame's text captures, by group name.</summary>
            public Dictionary<string, string> Patterns = [];

            /// <summary>The regex length of everything in the frame, its own bookends included - what decides whether a glyph's group is a call.</summary>
            public int Length;

            /// <summary>Whether the frame holds a call, so can't be inlined as text.</summary>
            public bool HasCalls;
        }

        abstract class Piece;
        sealed class TextPiece(string text, Dictionary<string, string> patterns) : Piece { public string Text = text; public Dictionary<string, string> Patterns = patterns; }
        sealed class CallPiece(string groupName, Quantifier? quantifier, MatchPlan target, string key) : Piece { public string GroupName = groupName; public Quantifier? Quantifier = quantifier; public MatchPlan Target = target; public string Key = key; }
        sealed class PlainPiece(Frame inner) : Piece { public Frame Inner = inner; }
        sealed class PipePiece : Piece;

        readonly Stack<Frame> _frames = new();
        readonly Frame _root;
        readonly GlyphNode _rootNode;

        public Collector(GlyphNode root)
        {
            _rootNode = root;
            _root = new() { Owner = root, Text = null };
            _frames.Push(_root);
        }

        /// <summary>The length of the flattened regex (see <see cref="BuiltRegex.MinifiedRegex"/>), with escaped spaces plain.</summary>
        public int UnescapedLength { get; private set; }

        /// <summary>The plan, once <see cref="Complete"/>d.</summary>
        public MatchPlan Plan { get; private set; }

        /// <summary>The base list is never kept: see the class remarks. The last content brick, which <see cref="RegexCollector.AlreadySeparated"/> reads, is.</summary>
        protected override bool KeepsBricks => false;

        public override void Append(RegexBrick brick)
        {
            base.Append(brick);
            UnescapedLength += BuiltRegex.UnescapedLength(brick.Regex);
            var frame = _frames.Peek();

            // A glyph's group opens a frame of its own; so does a plain group its owner puts around alternatives (see GlyphOneOfNode).
            if (brick is RegexBrickGroupOpen open && IsGlyphGroup(open))
            {
                _frames.Push(new() { Owner = (NamedGroupNode)brick.Parent, Open = open });
                Grow(brick);
                return;
            }

            if (IsPlain(brick, frame, "("))
            {
                _frames.Push(new() { Owner = frame.Owner });
                Grow(brick);
                return;
            }

            if (brick is RegexBrickGroupClose close && frame.Open is not null && brick.Parent == frame.Owner)
            {
                Grow(brick);
                _frames.Pop();
                CloseGroup(frame, QuantifierOf(close));
                return;
            }

            if (frame.Open is null && frame != _root && IsPlain(brick, frame, ")"))
            {
                Grow(brick);
                _frames.Pop();
                ClosePlain(frame);
                return;
            }

            Grow(brick);

            if (brick is RegexBrickJoiner { Joiner: Joiner.Pipe } && brick.Parent == frame.Owner)
            {
                FlushLeaf(frame);
                frame.Pieces.Add(new PipePiece());
                return;
            }

            AppendBrick(frame.Leaf, brick, frame.Patterns);
        }

        /// <summary>Finishes the root's plan.</summary>
        public void Complete()
        {
            if (_frames.Count != 1)
                throw new InvalidOperationException($"{_rootNode.FullyQualifiedName}: a group is opened and never closed");

            var prefix = _rootNode.FullyQualifiedName + "_";
            Plan = Shared(_rootNode.FullyQualifiedName + "/" + Digest(_root, prefix), () => ToPlan(_root, prefix));
        }

        static bool IsGlyphGroup(RegexBrickGroupOpen open) =>
            open.Parent is GlyphNode node && node is not DynamicGlyphNode && node.Navigation.IsGlyphType && node.ParentNode is not null;

        /// <summary>Whether <paramref name="brick"/> is a plain parenthesis <paramref name="frame"/>'s owner puts around alternatives (not a nib's text, which may start with one).</summary>
        static bool IsPlain(RegexBrick brick, Frame frame, string parenthesis) =>
            brick.Parent == frame.Owner && brick.GetType() == typeof(RegexBrick) && brick.Regex == parenthesis;

        /// <summary>Counts <paramref name="brick"/> toward every open frame, and keeps its text in the innermost while that might still be inlined.</summary>
        void Grow(RegexBrick brick)
        {
            foreach (var open in _frames)
                open.Length += brick.Regex.Length;

            var frame = _frames.Peek();

            // Once a glyph's group is large enough to be a call, its own text is never needed again.
            if (frame.Text is not null && frame.Open is not null && frame.Length >= CallThreshold)
                frame.Text = null;

            if (frame.Text is not null)
                AppendBrick(frame.Text, brick, frame.Patterns);
        }

        static void AppendBrick(StringBuilder text, RegexBrick brick, Dictionary<string, string> patterns)
        {
            if (brick.Parent is TextNode { AuthoredPattern: not null } node && brick.Regex == node.Text)
            {
                var name = PlanNode.PatternGroupPrefix + node.ParentNode.FullyQualifiedName + "_" + ((NamedGroupNode)node.ParentNode).Children.IndexOf(node);
                patterns[name] = node.AuthoredPattern;
                text.Append(node.WithPatternGroup(name));
            }
            else
                text.Append(brick.Regex);
        }

        static void FlushLeaf(Frame frame)
        {
            if (frame.Leaf.Length == 0)
                return;

            frame.Pieces.Add(new TextPiece(frame.Leaf.ToString(), frame.Patterns));
            frame.Leaf.Clear();
        }

        /// <summary>A glyph's group closed: inlined as text into the frame around it if small, else a call to its content's plan.</summary>
        void CloseGroup(Frame frame, Quantifier? quantifier)
        {
            var parent = _frames.Peek();

            if (frame.Text is not null && !frame.HasCalls)
            {
                Inline(frame, parent);
                return;
            }

            var prefix = frame.Open.GroupName + "_";
            var key = Digest(frame, prefix);
            var target = Shared(key, () => ToPlan(frame, prefix));
            FlushLeaf(parent);
            parent.Pieces.Add(new CallPiece(frame.Open.GroupName, quantifier, target, key));
            MarkCalls(parent);
        }

        /// <summary>A plain group closed: a level of its own in the frame around it if it holds a call, else text.</summary>
        void ClosePlain(Frame frame)
        {
            var parent = _frames.Peek();

            if (!frame.HasCalls)
            {
                Inline(frame, parent);
                return;
            }

            FlushLeaf(frame);
            FlushLeaf(parent);
            parent.Pieces.Add(new PlainPiece(frame));
            MarkCalls(parent);
        }

        /// <summary><paramref name="frame"/>'s whole text goes into <paramref name="parent"/>'s, as the flattened regex has it.</summary>
        static void Inline(Frame frame, Frame parent)
        {
            parent.Leaf.Append(frame.Text);
            parent.Text?.Append(frame.Text);

            foreach (var (name, pattern) in frame.Patterns)
                parent.Patterns[name] = pattern;
        }

        static void MarkCalls(Frame frame)
        {
            frame.HasCalls = true;
            frame.Text = null;
        }

        /// <summary>A digest of <paramref name="frame"/>'s content with names relative to <paramref name="prefix"/>: the same for the same content wherever it's nested.</summary>
        static string Digest(Frame frame, string prefix)
        {
            FlushLeaf(frame);
            var text = new StringBuilder();
            Describe(frame, prefix, text);
            return Convert.ToHexString(SHA256.HashData(Encoding.Unicode.GetBytes(text.ToString())));
        }

        static void Describe(Frame frame, string prefix, StringBuilder into)
        {
            foreach (var piece in frame.Pieces)
                switch (piece)
                {
                    case TextPiece text:
                        into.Append(RelativeText(text.Text, prefix));
                        break;

                    case CallPiece call:
                        into.Append("{call ").Append(RelativeName(call.GroupName, prefix)).Append(call.Quantifier?.GetDescription()).Append(':').Append(call.Key).Append('}');
                        break;

                    case PlainPiece plain:
                        FlushLeaf(plain.Inner);
                        into.Append("{(");
                        Describe(plain.Inner, prefix, into);
                        into.Append(")}");
                        break;

                    case PipePiece:
                        into.Append("{|}");
                        break;
                }
        }

        /// <summary><paramref name="frame"/>'s content as a plan node, names relative to <paramref name="prefix"/>.</summary>
        static PlanNode ToPlan(Frame frame, string prefix)
        {
            FlushLeaf(frame);
            var alternatives = new List<PlanNode>();
            var items = new List<PlanNode>();

            foreach (var piece in frame.Pieces)
                switch (piece)
                {
                    case TextPiece text:
                        items.Add(new PlanNode.Leaf(
                            RelativeText(text.Text, prefix).Replace(BuiltRegex.EscapedSpace, " "),
                            text.Patterns.ToDictionary(x => RelativeName(x.Key, prefix), x => x.Value)));
                        break;

                    case CallPiece call:
                        items.Add(new PlanNode.Call(RelativeName(call.GroupName, prefix), call.Quantifier, call.Target));
                        break;

                    case PlainPiece plain:
                        items.Add(ToPlan(plain.Inner, prefix));
                        break;

                    case PipePiece:
                        alternatives.Add(items.Count == 1 ? items[0] : new PlanNode.Sequence([.. items]));
                        items = [];
                        break;
                }

            alternatives.Add(items.Count == 1 ? items[0] : new PlanNode.Sequence([.. items]));
            return alternatives.Count == 1 ? alternatives[0] : new PlanNode.Alternation([.. alternatives]);
        }

        /// <summary><paramref name="text"/> with every group name under <paramref name="prefix"/> made relative to it: <c>(?&lt;prefix…&gt;</c> becomes <c>(?&lt;…&gt;</c>, and a pattern group's name likewise.</summary>
        static string RelativeText(string text, string prefix) =>
            text.Replace("(?<" + prefix, "(?<", StringComparison.Ordinal)
                .Replace("(?<" + PlanNode.PatternGroupPrefix + prefix, "(?<" + PlanNode.PatternGroupPrefix, StringComparison.Ordinal);

        /// <summary>A group's name (a pattern group's included) relative to <paramref name="prefix"/>.</summary>
        static string RelativeName(string name, string prefix)
        {
            if (name.StartsWith(PlanNode.PatternGroupPrefix, StringComparison.Ordinal))
                return PlanNode.PatternGroupPrefix + RelativeName(name[PlanNode.PatternGroupPrefix.Length..], prefix);

            return name.StartsWith(prefix, StringComparison.Ordinal)
                ? name[prefix.Length..]
                : throw new InvalidOperationException($"Group '{name}' isn't under '{prefix}'");
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
