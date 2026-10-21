namespace Glyphotype.RegexGeneration.Graph;

public class CaptureContext
{
    /// <summary>The match, held only while hydration may still look groups up in it - null once <see cref="Seal"/>ed.</summary>
    Match _match;

    /// <summary>For a match made by a plan (see <see cref="Matching.Matcher"/>): each group's captures, by the group's fully qualified name, in text order - held as <see cref="_match"/> is.</summary>
    Dictionary<string, List<(int Index, int Length)>> _captures;

    /// <summary>
    /// What each authored pattern matched within this match (see <see cref="Nodes.TextNode.AuthoredPattern"/>), in text
    /// order, in <see cref="SourceText"/>'s index space - a resolved dynamic's included (see <see cref="CaptureTrace.AdoptDynamicChildren"/>).
    /// Null for a match that didn't record them (one made from a regex <see cref="Match"/>).
    /// </summary>
    public List<PatternCapture> PatternCaptures { get; private set; }
    readonly Dictionary<string, CaptureTrace> _resolvedTraces = [];
    readonly Dictionary<(CaptureTrace child, CaptureTrace scope), CaptureTrace> _scopedViews = [];

    public RootCaptureTrace RootCaptureTrace { get; }
    public string SourceText { get; }
    public string FullMatch { get; }

    /// <summary>
    /// The <see cref="SourceText"/> index this match should be retried against as its new end, because a
    /// trailing <see cref="Nodes.DynamicGlyphNode"/> resolved less text than it captured (see
    /// <see cref="RequestNarrowedScopeEnd"/>) - or -1 when no narrowing was requested. Read by
    /// <see cref="RegexGraph.TryMatch(string, int, int, out Glyph)"/> once hydration against this context
    /// has run.
    /// </summary>
    public int NarrowedScopeEnd { get; private set; } = -1;

    /// <summary>
    /// Records that this match only genuinely accounts for source text up to <paramref name="scopeEnd"/>.
    /// Hydration against this context is already doomed by the time this is called; the request outlives
    /// that precisely because it's recorded here on the context rather than on the abandoned hydration
    /// result. The smallest request wins, so multiple trailing shortfalls in one pass narrow to the
    /// shortest span all of them agree on.
    /// </summary>
    public void RequestNarrowedScopeEnd(int scopeEnd) =>
        NarrowedScopeEnd = NarrowedScopeEnd < 0 ? scopeEnd : Math.Min(NarrowedScopeEnd, scopeEnd);

    /// <summary>The Tokenizer this match was made by, which any <see cref="Nodes.DynamicGlyphNode"/> in it resolves through - or null for a match made outside any tokenization.</summary>
    public Tokenizer Tokenizer { get; }

    public CaptureContext(GlyphNode rootNode, Match match, string sourceText, Tokenizer tokenizer = null)
    {
        Tokenizer = tokenizer;
        _match = match;
        SourceText = sourceText;
        FullMatch = match.Value;
        RootCaptureTrace = new(this, rootNode, match);
    }

    /// <summary>
    /// The context of a match <paramref name="rootNode"/>'s plan made (see <see cref="Matching.Matcher"/>), spanning <paramref name="length"/>
    /// characters of <paramref name="sourceText"/> from <paramref name="index"/>, with <paramref name="captures"/> as the plan reported them:
    /// group names relative to the root, which this qualifies.
    /// </summary>
    public CaptureContext(GlyphNode rootNode, string sourceText, int index, int length, List<Matching.PlanCapture> captures, Tokenizer tokenizer = null)
    {
        Tokenizer = tokenizer;
        SourceText = sourceText;
        FullMatch = sourceText.Substring(index, length);
        _captures = [];
        PatternCaptures = [];
        var prefix = rootNode.FullyQualifiedName + "_";

        foreach (var capture in captures)
        {
            if (capture.Pattern is not null)
            {
                // An optional pattern that matched nothing is left out, as the regex's own pattern groups leave it out.
                if (capture.Length > 0)
                    PatternCaptures.Add(new(capture.Pattern, capture.Index, capture.Length));

                continue;
            }

            if (!_captures.TryGetValue(prefix + capture.Name, out var occurrences))
                _captures[prefix + capture.Name] = occurrences = [];

            occurrences.Add((capture.Index, capture.Length));
        }

        PatternCaptures.Sort((a, b) => a.Index.CompareTo(b.Index));
        RootCaptureTrace = new(this, rootNode, index, length);
    }

    /// <summary>Adds <paramref name="inner"/>'s pattern captures - a resolved dynamic's, in the index space of its own text - shifted by <paramref name="offset"/> into this context's.</summary>
    internal void AdoptPatternCaptures(CaptureContext inner, int offset)
    {
        if (inner.PatternCaptures is null || inner.PatternCaptures.Count == 0)
            return;

        PatternCaptures ??= [];
        PatternCaptures.AddRange(inner.PatternCaptures.Select(x => x with { Index = x.Index + offset }));
        PatternCaptures.Sort((a, b) => a.Index.CompareTo(b.Index));
    }

    /// <summary>
    /// Lets go of the match: hydration is done, every group it resolved is a trace of its own (which copies what it
    /// needs), and a group nothing resolved is one that matched nothing as far as any later reader is concerned.
    /// A match of a regex with thousands of named groups holds arrays sized by that count, so keeping one per
    /// token for a whole corpus is most of the corpus's memory.
    /// </summary>
    public void Seal()
    {
        _match = null;
        _captures = null;
    }

    public CaptureTrace this[NamedGroupNode namedGroupNode]
    {
        get
        {
            // Idempotent by design: multiple nodes in the hydration path (e.g. SetPropertyValue
            // followed by a nested TryHydrate) may look up the same group. Re-resolving would
            // re-register a second, duplicate child on the parent trace, so cache per FQN.
            if (_resolvedTraces.TryGetValue(namedGroupNode.FullyQualifiedName, out var cached))
                return cached;

            // Looked up on demand, for just the groups hydration asks about: materializing every named group of a
            // regex with thousands of them, per match, is what made each token cost hundreds of kilobytes.
            (int Index, int Length)[] allCapturesForGroup = [];

            if (_match is not null && _match.Groups[namedGroupNode.FullyQualifiedName] is { Success: true } group)
                allCapturesForGroup = group.Captures.Cast<Capture>().Select(x => (x.Index, x.Length)).ToArray();
            else if (_captures is not null && _captures.TryGetValue(namedGroupNode.FullyQualifiedName, out var occurrences))
                allCapturesForGroup = [.. occurrences];

            if (allCapturesForGroup.Length == 0)
            {
                // Equivalent to "no capture found" (not necessarily an exception condition unless the caller expects one or more captures)
                var emptyTrace = new CaptureTrace(this, namedGroupNode);
                _resolvedTraces[namedGroupNode.FullyQualifiedName] = emptyTrace;
                return emptyTrace;
            }

            var captureTraces = allCapturesForGroup
                .Select((x, idx) => new CaptureTrace(this, namedGroupNode, x.Index, x.Length, allCapturesForGroup.Length == 1 ? null : idx))
                .ToList();

            var captureTrace = captureTraces[0];

            if (captureTraces.Count > 1)
            {
                var siblings = captureTraces.Skip(1).ToList();
                captureTrace.Siblings.AddRange(siblings);

                // Hydration only ever recurses into and registers Children for this FIRST-seen
                // occurrence (below) - every later occurrence needs to know that, so a display-time
                // walk of its own effective children (see CaptureTrace.EffectiveChildren) can borrow
                // this one's real Children instead of finding its own (permanently empty) list.
                foreach (var sibling in siblings)
                    sibling.SetRepresentative(captureTrace);
            }

            RootCaptureTrace.AddCaptureTrace(captureTrace);
            _resolvedTraces[namedGroupNode.FullyQualifiedName] = captureTrace;

            return captureTrace;
        }
    }

    /// <summary>
    /// Memoizes <see cref="CaptureTrace.WithinScope"/>'s narrowed view of <paramref name="child"/> for
    /// <paramref name="scope"/>, keyed by value equality on both (see <see cref="CaptureTrace.Equals"/>) -
    /// so two separate calls that narrow the exact same child to the exact same repetition (e.g. once
    /// while computing a line's palette, again while rendering it) get back the exact same object,
    /// instead of two freshly-copied views that look alike but neither carries whatever the other one
    /// (like hydration's own <see cref="CaptureTrace.ClrValue"/> assignment) wrote onto it.
    /// </summary>
    internal CaptureTrace GetOrCreateScopedView(CaptureTrace child, CaptureTrace scope, Func<CaptureTrace> factory)
    {
        var key = (child, scope);

        if (_scopedViews.TryGetValue(key, out var cached))
            return cached;

        var view = factory();
        _scopedViews[key] = view;
        return view;
    }

    public override string ToString() => FullMatch;
}