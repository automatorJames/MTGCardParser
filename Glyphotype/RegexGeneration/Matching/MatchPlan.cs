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

    /// <summary>A plain group around <see cref="Inner"/>: matches as it does, and keeps the pattern's shape when the plan is written out again.</summary>
    public sealed class Group(PlanNode inner) : PlanNode
    {
        public PlanNode Inner { get; } = inner;
    }

    /// <summary>A nested glyph's group, matched by its own shared plan, and captured under <see cref="Name"/>.</summary>
    public sealed class Call(string name, Quantifier? quantifier, MatchPlan target) : PlanNode
    {
        /// <summary>The group's name, relative to the plan's root - or null for a call that captures nothing of its own: a type's plan standing for a group's content, within that group's own plan.</summary>
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

    /// <summary>As <see cref="Shared"/>, with the key the plan was found or built under.</summary>
    static (MatchPlan Plan, string Key) SharedWithKey(string key, Func<PlanNode> build)
    {
        var plan = Shared(key, build);
        return (plan, plan.Key);
    }

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

    /// <summary>The plan written out as one pattern again, every call expanded in place (names as the plan holds them, relative to each called group) - what two ways of building a type's plan can be compared on.</summary>
    internal string Render()
    {
        var text = new StringBuilder();
        Render(Root, text);
        return text.ToString();
    }

    static void Render(PlanNode node, StringBuilder into)
    {
        switch (node)
        {
            case PlanNode.Leaf leaf:
                into.Append(leaf.Pattern);
                break;

            case PlanNode.Sequence sequence:
                foreach (var item in sequence.Items)
                    Render(item, into);
                break;

            case PlanNode.Alternation alternation:
                for (var i = 0; i < alternation.Alternatives.Length; i++)
                {
                    if (i > 0)
                        into.Append('|');

                    Render(alternation.Alternatives[i], into);
                }
                break;

            case PlanNode.Group group:
                into.Append('(');
                Render(group.Inner, into);
                into.Append(')');
                break;

            case PlanNode.Call call:
                if (call.Name is null)
                    Render(call.Target.Root, into);
                else
                {
                    into.Append("(?<").Append(call.Name).Append('>');
                    Render(call.Target.Root, into);
                    into.Append(')').Append(call.Quantifier is Quantifier.Optional ? "?" : call.Quantifier is Quantifier.AnyNumber ? "*" : "");
                }
                break;
        }
    }

    static int Count(PlanNode node) => node switch
    {
        PlanNode.Sequence sequence => sequence.Items.Sum(Count),
        PlanNode.Alternation alternation => alternation.Alternatives.Sum(Count),
        PlanNode.Group group => Count(group.Inner),
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

        /// <param name="groupName">The group's name - or null for a call standing for a group's content within the group's own frame, which captures nothing of its own.</param>
        sealed class CallPiece(string groupName, Quantifier? quantifier, MatchPlan target, string key) : Piece { public string GroupName = groupName; public Quantifier? Quantifier = quantifier; public MatchPlan Target = target; public string Key = key; }
        sealed class PlainPiece(Frame inner) : Piece { public Frame Inner = inner; }
        sealed class PipePiece : Piece;

        readonly Stack<Frame> _frames = new();
        readonly Frame _root;
        readonly GlyphNode _rootNode;

        /// <param name="compositional">Whether large nested glyphs are called (see <see cref="Calls"/>) - false walks every group, as a check on the calls.</param>
        public Collector(GlyphNode root, bool compositional = true)
        {
            _rootNode = root;
            _compositional = compositional;
            _root = new() { Owner = root, Text = null };
            _frames.Push(_root);
        }

        readonly bool _compositional;

        /// <summary>For checking the calls: the text the collector accounts for, each call standing in as its type's written pattern - what should equal the written pattern. Kept only when asked (see <see cref="TraceText"/>).</summary>
        internal StringBuilder TraceText { get; set; }

        /// <summary>The length of the flattened regex (see <see cref="BuiltRegex.MinifiedRegex"/>), with escaped spaces plain.</summary>
        public int UnescapedLength { get; private set; }

        /// <summary>The length of the flattened regex as its bricks spell it.</summary>
        public int RawLength { get; private set; }

        /// <summary>How many named groups the flattened regex has, at every depth - what its length grows by per character of the name of a group it's nested under (see <see cref="AppendCall"/>).</summary>
        public int GroupCount { get; private set; }

        /// <summary>What the pattern's content ends in (see <see cref="RegexCollector.LastContentRegex"/>).</summary>
        public string LastContent => LastContentRegex;

        /// <summary>
        /// A nested glyph's group whose type's pattern is large is called, not walked: its content is the type's own plan,
        /// shared by every type nesting it, and the inlined graph beneath the group is never built for matching's sake.
        /// </summary>
        public override bool Calls(NamedGroupNode group) =>
            _compositional && IsGlyphGroup(group) && IsLarge(group) && !VariesByProperty(group);

        /// <summary>
        /// Whether <paramref name="group"/>'s content depends on the property it's bound to, so its type's own plan can't stand for
        /// it: a list property's <see cref="JoinedByAttribute"/> sets the separator between its items (see <see cref="JoinedItemNode"/>).
        /// Such a group is walked; the items within it are called as usual.
        /// </summary>
        static bool VariesByProperty(NamedGroupNode group) =>
            group.Navigation.Prop?.GetCustomAttribute<JoinedByAttribute>() is not null;

        /// <summary>Whether <paramref name="group"/>'s type's pattern reaches <see cref="CallThreshold"/> - the one measure of "large", so a group is a call or inlined alike however its plan is built.</summary>
        static bool IsLarge(NamedGroupNode group) =>
            GlyphTypeCache.GetRegexGraph(group.Navigation.NodeType).BuiltRegex.RawLength >= CallThreshold;

        public override void AppendCall(NamedGroupNode group)
        {
            var called = GlyphTypeCache.GetRegexGraph(group.Navigation.NodeType);
            var pattern = called.BuiltRegex;

            // Inlined, every group within the called pattern is named under this group rather than under the called type's
            // root, so the pattern as inlined is longer (or shorter) by that difference per group - and the length is what
            // orders types for the tokenizer, so it's counted as the inlined pattern would be.
            var renamed = group.FullyQualifiedName.Length - called.RootNode.FullyQualifiedName.Length;
            UnescapedLength += pattern.MinifiedRegexLength + pattern.NamedGroupCount * renamed;
            RawLength += pattern.RawLength + pattern.NamedGroupCount * renamed;
            GroupCount += pattern.NamedGroupCount;
            TraceText?.Append(pattern.MinifiedRegex.Replace("(?<" + called.RootNode.FullyQualifiedName + "_", "(?<" + group.FullyQualifiedName + "_", StringComparison.Ordinal));

            foreach (var open in _frames)
                open.Length += pattern.RawLength + pattern.NamedGroupCount * renamed;

            if (pattern.LastContentRegex is not null)
                NoteContent(pattern.LastContentRegex);

            var frame = _frames.Peek();
            FlushLeaf(frame);
            frame.Pieces.Add(new CallPiece(null, null, pattern.Plan, pattern.Plan.Key));
            MarkCalls(frame);
        }

        /// <summary>The plan, once <see cref="Complete"/>d.</summary>
        public MatchPlan Plan { get; private set; }

        /// <summary>The base list is never kept: see the class remarks. The last content brick, which <see cref="RegexCollector.AlreadySeparated"/> reads, is.</summary>
        protected override bool KeepsBricks => false;

        public override void Append(RegexBrick brick)
        {
            base.Append(brick);
            UnescapedLength += BuiltRegex.UnescapedLength(brick.Regex);
            RawLength += brick.Regex.Length;

            if (brick is RegexBrickGroupOpen)
                GroupCount++;

            TraceText?.Append(brick.Regex.Replace(BuiltRegex.EscapedSpace, " "));
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
            Plan = Shared((_compositional ? "c:" : "w:") + _rootNode.FullyQualifiedName + "/" + Digest(_root, prefix), () => ToPlan(_root, prefix));
        }

        static bool IsGlyphGroup(RegexBrickGroupOpen open) =>
            open.Parent is NamedGroupNode node && IsGlyphGroup(node);

        /// <summary>Whether <paramref name="node"/> is a nested glyph's group - one with a type whose own plan could stand for its content.</summary>
        static bool IsGlyphGroup(NamedGroupNode node) =>
            node is GlyphNode and not DynamicGlyphNode && node.Navigation.IsGlyphType && node.ParentNode is not null;

        /// <summary>Whether <paramref name="brick"/> is a plain parenthesis <paramref name="frame"/>'s owner puts around alternatives (not a nib's text, which may start with one).</summary>
        static bool IsPlain(RegexBrick brick, Frame frame, string parenthesis) =>
            brick.Parent == frame.Owner && brick.GetType() == typeof(RegexBrick) && brick.Regex == parenthesis;

        /// <summary>Counts <paramref name="brick"/> toward every open frame, and keeps its text in the innermost while that might still be inlined.</summary>
        void Grow(RegexBrick brick)
        {
            foreach (var open in _frames)
                open.Length += brick.Regex.Length;

            var frame = _frames.Peek();

            // Once a glyph's group is known to be large enough to be a call, its own text is never needed again.
            if (frame.Text is not null && frame.Open is not null && frame.Length >= CallThreshold && IsLarge(frame.Owner))
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

            if (!IsLarge(frame.Owner) && !frame.HasCalls)
            {
                Inline(frame, parent);
                return;
            }

            var prefix = frame.Open.GroupName + "_";
            FlushLeaf(frame);

            // A group whose content is nothing but a call to its type's plan is that plan.
            var (target, key) = frame.Pieces is [CallPiece { GroupName: null } only]
                ? (only.Target, only.Key)
                : SharedWithKey((_compositional ? "c:" : "w:") + Digest(frame, prefix), () => ToPlan(frame, prefix));

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
                        into.Append("{call ").Append(call.GroupName is null ? "" : RelativeName(call.GroupName, prefix)).Append(call.Quantifier?.GetDescription()).Append(':').Append(call.Key).Append('}');
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
                        items.Add(new PlanNode.Call(call.GroupName is null ? null : RelativeName(call.GroupName, prefix), call.Quantifier, call.Target));
                        break;

                    case PlainPiece plain:
                        items.Add(new PlanNode.Group(ToPlan(plain.Inner, prefix)));
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
