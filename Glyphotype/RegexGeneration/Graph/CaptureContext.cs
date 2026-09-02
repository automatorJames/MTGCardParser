namespace Glyphotype.RegexGeneration.Graph;

public class CaptureContext
{
    IReadOnlyDictionary<string, Capture[]> _captureDictionary;
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

    public CaptureContext(GlyphNode rootNode, Match match, string sourceText)
    {
        _captureDictionary = GetNamedGroupCaptures(match);
        SourceText = sourceText;
        FullMatch = match.Value;
        RootCaptureTrace = new(this, rootNode, match);
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

            if (!_captureDictionary.TryGetValue(namedGroupNode.FullyQualifiedName, out var allCapturesForGroup))
                // This should never happen; even if an FQN has no captures, it should still appear in the dictionary
                throw new Exception($"Name '{namedGroupNode.FullyQualifiedName}' does not appear in the dictionary");

            if (allCapturesForGroup.Length == 0)
            {
                // Equivalent to "no capture found" (not necessarily an exception condition unless the caller expects one or more captures)
                var emptyTrace = new CaptureTrace(this, namedGroupNode);
                _resolvedTraces[namedGroupNode.FullyQualifiedName] = emptyTrace;
                return emptyTrace;
            }

            var captureTraces = allCapturesForGroup
                .Select((x, idx) => new CaptureTrace(this, namedGroupNode, x, allCapturesForGroup.Length == 1 ? null : idx))
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

    public CaptureTrace GetResolvedTrace(string groupName)
    {
        if (_resolvedTraces.TryGetValue(groupName, out var trace))
            return trace;

        throw new Exception($"No resolved trace for grop name {groupName}");
    }

    static Dictionary<string, Capture[]> GetNamedGroupCaptures(Match match)
    {
        if (match == null || !match.Success) return new();

        var regex = (Regex)typeof(Match)
            .GetField("_regex", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(match);

        if (regex == null) return new();

        return regex.GetGroupNames()
            .Where(name => !int.TryParse(name, out _))
            .ToDictionary(
                name => name,
                name => match.Groups[name].Captures.Cast<Capture>().ToArray()
            );
    }

    public override string ToString() => FullMatch;
}