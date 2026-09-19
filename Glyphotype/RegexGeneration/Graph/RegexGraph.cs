using System.Diagnostics;

namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// The compiled regex-matching representation of one root <see cref="Glyph"/> type: the
/// <see cref="Graph.Nodes.RegexNode"/> tree walked from that type, the <see cref="BuiltRegex"/> compiled
/// from it, and lookup tables (<see cref="NamedGroupFlatGraph"/>, <see cref="SimpleUniqueNames"/>) used
/// to resolve captures and simplified display names.
/// </summary>
public class RegexGraph
{
    static readonly char[] _boundaryChars = [' ', '.'];

    /// <summary>The root <see cref="Glyph"/> type this graph was built from.</summary>
    public Type RootGlyphType { get; }

    /// <summary>
    /// Flag indicating whether the Glyph type has the [Dependent] attribute, meaning it should only be considered
    /// as part of top level Glyph graph instead of being a top level graph itself.
    /// </summary>
    public bool IsDependent { get; }

    /// <summary>The root of the walked <see cref="Graph.Nodes.RegexNode"/> tree.</summary>
    public GlyphNode RootNode { get; }

    /// <summary>The compiled matching regex produced by walking <see cref="RootNode"/>.</summary>
    public BuiltRegex BuiltRegex { get; }

    /// <summary>
    /// Whether <see cref="RootGlyphType"/> carries <see cref="MustMatchWholeLineAttribute"/>: this type
    /// claims an entire line or nothing, so it is only ever a candidate at the tokenization scope's own
    /// start, and its match has to run all the way to the scope's end. The <em>strictest</em> of the three
    /// span rules - see the table on <see cref="AllowsPartialSegmentMatch"/>.
    /// <para>
    /// Enforced here rather than by the Tokenizer, since it's a fact about the type rather than about
    /// where the cursor happens to be: <see cref="TryMatch(string, int, int, out Glyph, bool)"/> applies
    /// it to every call regardless of what the caller asked for. The Tokenizer only contributes the
    /// candidacy half (skipping the type once the cursor has left the scope start), which it can't learn
    /// from here.
    /// </para>
    /// </summary>
    public bool MustMatchWholeLine { get; }

    /// <summary>
    /// Whether <see cref="RootGlyphType"/> carries <see cref="AllowPartialSegmentMatchAttribute"/>: this
    /// type may stop partway through a clause, so it is exempt from the whole-segment requirement the
    /// Tokenizer otherwise imposes when <see cref="GlobalSettings.AllowPartialSegmentMatches"/> is false.
    /// The <em>loosest</em> of the three span rules.
    /// <para>
    /// The three are answers to two different questions, which is why they aren't points on one scale.
    /// <c>MustMatchWholeLine</c> and this one answer "how much must a match cover?"; they are opposite
    /// ends of that axis and are mutually exclusive (enforced in <see cref="Glyph.ValidateStructure"/>).
    /// <see cref="SpansClauses"/> answers a different question - "may a match cross a period at all?" -
    /// and only has anything to say in the default case between them:
    /// </para>
    /// <list type="table">
    /// <listheader><term>rule</term><description>must start at / must end at</description></listheader>
    /// <item><term>MustMatchWholeLine</term><description>scope start / scope end - every clause on the line, always, whatever the global setting says</description></item>
    /// <item><term>(default)</term><description>clause start / first clause boundary - exactly one clause, or with <see cref="SpansClauses"/>, any whole number of them</description></item>
    /// <item><term>AllowsPartialSegmentMatch</term><description>anywhere / any word boundary - the pre-setting behavior, kept per-type</description></item>
    /// </list>
    /// <para>
    /// Unlike <see cref="MustMatchWholeLine"/>, nothing in this class reads this one: exempting a type
    /// just means the Tokenizer hands it the ordinary scope and asks for an ordinary match, so there's no
    /// rule left here to apply. It's purely a statement about candidacy, and only meaningful while the
    /// global setting is false - with the setting true, every type already matches this way.
    /// </para>
    /// </summary>
    public bool AllowsPartialSegmentMatch { get; }

    /// <summary>
    /// Whether <see cref="RootGlyphType"/> declares a bare <c>"."</c> nib anywhere in its graph, i.e.
    /// states outright that it spans a clause boundary. Such a type may end at <em>any</em> clause
    /// boundary rather than being capped at the first - the Tokenizer offers it each successive one in
    /// turn, shortest first, so it settles on the fewest clauses that satisfy it.
    /// <para>
    /// Orthogonal to the two attributes above, not a third point on their scale: they set how much a match
    /// must cover, this sets whether it may cross a period while doing so. It only changes anything in the
    /// default case, since <see cref="MustMatchWholeLine"/> already crosses every period on the line and
    /// <see cref="AllowsPartialSegmentMatch"/> opts out of clause accounting altogether.
    /// </para>
    /// <para>
    /// Keyed off an explicit period nib rather than "may this pattern contain a period", because the
    /// latter would hand every greedy wildcard a licence to swallow whole clauses. A type only gets to
    /// cross a period when it says so.
    /// </para>
    /// </summary>
    public bool SpansClauses { get; }

    /// <summary>
    /// Maps NamedGroupNode FullyQualifiedName -> RegexNode.
    /// </summary>
    public Dictionary<string, NamedGroupNode> NamedGroupFlatGraph { get; } = [];

    /// <summary>
    /// Maps each NamedGroupNode FullyQualifiedName to a minimum unique simplified
    /// name. The simplified name is typically the name of the node itself
    /// disambiguation is required. For example, "My_Path_To_Node" -> "Node".
    /// </summary>
    public Dictionary<string, string> SimpleUniqueNames { get; } = [];

    public RegexGraph(Type rootGlyphType, GlyphNode rootNode)
    {
        RootGlyphType = rootGlyphType;
        IsDependent = rootGlyphType.IsDefined(typeof(DependentAttribute));
        RootNode = rootNode;
        MustMatchWholeLine = rootGlyphType.IsDefined(typeof(MustMatchWholeLineAttribute));
        AllowsPartialSegmentMatch = rootGlyphType.IsDefined(typeof(AllowPartialSegmentMatchAttribute));
        SpansClauses = ContainsClauseBreak(rootNode);
        RegexCollector collector = new();
        RootNode.AppendRegexBricks(collector);
        BuiltRegex = collector.GetBuiltRegex();
        PopulateFlatGraphRecursive();
        PopulateSimpleUniqueNames();
    }

    /// <summary>Whether <paramref name="node"/>'s subtree contains a bare-period <see cref="TextNode"/> - see <see cref="SpansClauses"/>.</summary>
    static bool ContainsClauseBreak(NamedGroupNode node) =>
        node.Children.Any(x => x is TextNode { IsClauseBreak: true })
        || node.Children.OfType<NamedGroupNode>().Any(ContainsClauseBreak);

    /// <summary>Builds the root <see cref="Graph.Nodes.RegexNode"/> for <paramref name="rootGlyphType"/> and compiles a full <see cref="RegexGraph"/> from it.</summary>
    public static RegexGraph Create(Type rootGlyphType)
    {
        Navigation navigation = new(rootGlyphType);
        var root = GlyphNode.GetNodeForNavigaton(null, navigation);

        if (root is not GlyphNode glyphNodeRoot)
            throw new Exception($"Expected a {nameof(GlyphNode)}, but got a {root.GetType().Name}");

        return new(rootGlyphType, glyphNodeRoot);
    }

    /// <summary>
    /// The positional rainbow palette for every named group in this graph, in <see cref="NamedGroupFlatGraph"/>'s
    /// declaration order (transparent root first) - with the leading <paramref name="positionalOverrideColors"/>
    /// positions pinned to a fixed color instead of a rainbow hue (e.g. a neutral color for the never-boxed
    /// root). The one place every "one fixed color per named group" consumer (the formatted regex's own
    /// coloring, TypeTreeView's boxes) gets its base ordering from, so they only ever differ by choice of
    /// override color, never by a re-derived named-group ordering.
    /// </summary>
    public Dictionary<NamedGroupNode, HexPalette> GetNamedGroupPaletteSet(params HexColor[] positionalOverrideColors) =>
        DeterministicPalette.GetPositionalPaletteSet(NamedGroupFlatGraph.Values, positionalOverrideColors);

    /// <summary>
    /// Same as <see cref="GetNamedGroupPaletteSet(HexColor[])"/>, but appended with any further named
    /// groups that only show up via <paramref name="extraBricks"/> - typically a dynamic capture's
    /// resolved sub-type, spliced in by <see cref="Presentation.DynamicSectionBuilder"/> once actual
    /// occurrence data is available to expand it, which this static graph never declared on its own. Each
    /// gets its own further rainbow slot, in first-appearance order, joining the same rainbow as the base
    /// set rather than starting a new one - the exact named-group ordering <see cref="Presentation.SmartLineRenderer"/>
    /// colors a formatted regex with.
    /// </summary>
    public Dictionary<NamedGroupNode, HexPalette> GetNamedGroupPaletteSet(IEnumerable<RegexBrick> extraBricks, params HexColor[] positionalOverrideColors)
    {
        var namedGroupsInDisplayOrder = NamedGroupFlatGraph.Values
            .Concat(extraBricks.Select(x => x.NamedGroupParent).Where(x => x is not null))
            .Distinct();

        return DeterministicPalette.GetPositionalPaletteSet(namedGroupsInDisplayOrder, positionalOverrideColors);
    }

    /// <summary>Depth-first walk populating <see cref="NamedGroupFlatGraph"/> from every named group node in the tree.</summary>
    void PopulateFlatGraphRecursive(NamedGroupNode node = null)
    {
        node ??= RootNode;
        NamedGroupFlatGraph[node.FullyQualifiedName] = node;

        foreach (var child in node.Children.OfType<NamedGroupNode>())
            PopulateFlatGraphRecursive(child);
    }

    /// <summary>Computes <see cref="SimpleUniqueNames"/> by growing each name's suffix (shortest first) until it uniquely identifies that node among all others.</summary>
    void PopulateSimpleUniqueNames()
    {
        foreach (var fullyQualifiedName in NamedGroupFlatGraph.Keys)
        {
            var parts = fullyQualifiedName.Split('_');

            SimpleUniqueNames[fullyQualifiedName] = Enumerable
                .Range(1, parts.Length)
                .Select(partCount => string.Join('_', parts[^partCount..]))
                .First(candidate => NamedGroupFlatGraph.Keys.Count(
                    key => key == candidate || key.EndsWith($"_{candidate}")) == 1);
        }
    }

    /// <summary>Attempts to match and hydrate <paramref name="sourceText"/> in full, from its start to its end.</summary>
    public bool TryMatch(string sourceText, out Glyph glyph) =>
        TryMatch(sourceText, 0, sourceText.Length, out glyph);

    /// <summary>
    /// Evaluates if the source text at the current index satisfies the regex and MTG boundary rules.
    /// </summary>
    /// <param name="mustConsumeWholeScope">
    /// Forces the whole-scope rule described on <see cref="MustMatchWholeLine"/> onto this one call, for a
    /// type that doesn't carry <see cref="MustMatchWholeLineAttribute"/> itself. How the Tokenizer imposes
    /// its whole-segment requirement: it narrows <paramref name="endIndex"/> to the end of the current
    /// segment and then demands the match fill it, which is the same shape of rule against a smaller scope.
    /// </param>
    public bool TryMatch(string sourceText, int currentIndex, int endIndex, out Glyph glyph, bool mustConsumeWholeScope = false)
    {
        // Retried against a progressively shorter scope whenever hydration discovers that a trailing
        // DynamicGlyph resolved less text than its greedy pattern captured (see
        // DynamicGlyphNode.TryHydrate) - so what this returns is a match whose whole span really is
        // accounted for, which is what lets Tokenizer safely resume at the first character past it.
        // Re-running the entire match, rather than trimming back the capture tree already built from the
        // longer one, is what keeps the resulting CaptureContext internally consistent: the compiled
        // Match, every capture under it, and every index they carry all describe the same span. Each
        // retry strictly shortens the scope, so this terminates.
        int scopeEnd = endIndex;

        while (true)
        {
            if (TryMatchWithinScope(sourceText, currentIndex, endIndex, scopeEnd, mustConsumeWholeScope, out glyph, out int narrowedScopeEnd))
                return true;

            // Either no narrowing was requested (an ordinary failed match, leaving -1) or the one that
            // was wouldn't actually make progress - nothing further to try in both cases.
            if (narrowedScopeEnd <= currentIndex || narrowedScopeEnd >= scopeEnd)
                return false;

            scopeEnd = narrowedScopeEnd;
        }
    }

    /// <summary>One attempt of <see cref="TryMatch(string, int, int, out Glyph)"/>.</summary>
    /// <param name="scopeEnd">
    /// The end of the window the regex itself may consume. Equal to <paramref name="endIndex"/> on the
    /// first attempt, and shorter only on a narrowed retry. The window is bounded up front in two cases:
    /// on a narrowed retry, so a greedy pattern can't just re-take the very text the retry exists to
    /// exclude; and whenever <paramref name="mustConsumeWholeScope"/> is set, so that a greedy pattern
    /// which would otherwise overshoot the scope gets backtracked into filling it rather than matching
    /// past it and being thrown out by the range check. An ordinary partial-match attempt still runs
    /// unbounded and is range-checked afterwards, exactly as it always has - there, overshooting really
    /// is a failure rather than something to backtrack out of.
    /// </param>
    /// <param name="mustConsumeWholeScope"><inheritdoc cref="TryMatch(string, int, int, out Glyph, bool)" path="/param[@name='mustConsumeWholeScope']"/></param>
    /// <param name="narrowedScopeEnd">The scope end to retry at, or -1 if no narrowing was requested.</param>
    bool TryMatchWithinScope(string sourceText, int currentIndex, int endIndex, int scopeEnd, bool mustConsumeWholeScope, out Glyph glyph, out int narrowedScopeEnd)
    {
        glyph = null;
        narrowedScopeEnd = -1;

        // MustMatchWholeLine imposes the same "fill the scope" requirement the per-call flag does, so it
        // gets the same bounded window - otherwise a greedy whole-line type would overshoot and be thrown
        // out by the range check instead of being backtracked into filling the line.
        bool mustFillScope = MustMatchWholeLine || mustConsumeWholeScope;

        var match = scopeEnd == endIndex && !mustFillScope
            ? BuiltRegex.Regex.Match(sourceText, currentIndex)
            : BuiltRegex.Regex.Match(sourceText, currentIndex, scopeEnd - currentIndex);

        int matchEndIndex = match.Index + match.Length;

        // A MustMatchWholeLine type is a special case: nothing else may share its tokenization pass, so
        // it must consume the entire requested scope - ending at a boundary char partway through isn't
        // good enough. mustConsumeWholeScope asks for that same treatment per-call, which is how the
        // Tokenizer enforces its whole-segment requirement against a segment-sized endIndex. Every other
        // type keeps the normal "end of scope, or followed by a boundary char" partial-match allowance.
        bool endsAtBoundary = mustFillScope
            ? matchEndIndex == endIndex
            : matchEndIndex == endIndex || (matchEndIndex < endIndex && _boundaryChars.Contains(sourceText[matchEndIndex]));

        bool matchIsValid =
            match.Success                   // 1. Regex:        Match must be successful
            && match.Index == currentIndex  // 2. Anchoring:    Must start exactly at currentIndex
            && match.Length > 0             // 3. Length:       Must be non-empty
            && matchEndIndex <= endIndex    // 4. Scope:        Must not exceed endIndex
            && endsAtBoundary;              // 5. Boundary:     Must end at end index, or a boundary char, or if applicable match whole line

        if (!matchIsValid)
            return false;

        CaptureContext captureContext = new(RootNode, match, sourceText);
        var success = RootNode.TryHydrate(captureContext.RootCaptureTrace, out glyph);
        narrowedScopeEnd = captureContext.NarrowedScopeEnd;

        // A narrowing request outranks hydration's own verdict: a Glyph that hydrated fine despite one
        // (because the shortfalling DynamicGlyph sat on a nullable property, so its own failure didn't
        // fail its parent) would still be claiming text that no DynamicGlyph ever accounted for.
        if (!success || narrowedScopeEnd >= 0)
        {
            glyph = null;
            return false;
        }

        captureContext.RootCaptureTrace.ClrValue = glyph;
        return true;
    }
}
