namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// The centralized "control panel" of color knobs for every <see cref="TokenRegexSpanKind"/>, and optionally
/// per <see cref="CaptureNodeType"/> for roles that want it (currently just <see cref="TokenRegexSpanKind.GroupBorderWall"/>,
/// distinguishing e.g. an Enum group's border from a TokenUnit group's). This is the single place to retune
/// how TypeRegexPage colors any of these span roles; nothing outside this class should hardcode one of
/// their colors. Sits alongside <see cref="SmartRegexStaticRules"/> as the other "knobs" class for formatted
/// regex output — that one governs layout, this one governs color.
/// </summary>
/// <remarks>
/// A role's hue is always whatever the caller passes into <see cref="Resolve"/> as
/// <c>positionalHueDegrees</c> — in practice, the same rainbow hue
/// <see cref="DeterministicPalette.GetPositionalPaletteSet{T}"/> already assigned to the span's enclosing
/// named group. That's deliberate for every role, including <see cref="TokenRegexSpanKind.GroupBorderWall"/>:
/// nested boxes read best when each box's wall is its own container-meaningful color, so a border's hue
/// should never be pinned regardless of which group it belongs to. Its per-<see cref="CaptureNodeType"/>
/// entries below only vary saturation/brightness (bolder for an Enum's solid box, more muted for a
/// TokenUnit's dashed one) — the hue is still always the container's own.
/// </remarks>
public static class SmartSpanColorPanel
{
    /// <summary>Per-role knobs. <see cref="ColorKnobs.HueDegrees"/> is ignored here — it's always overwritten with the span's positional rainbow hue in <see cref="Resolve"/> — so only saturation/brightness (and their hi/lo ranges) are hand-tuned per role.</summary>
    static readonly Dictionary<TokenRegexSpanKind, ColorKnobs> _roleSpecs = new()
    {
        [TokenRegexSpanKind.EnumMemberRegex] = new(),
        [TokenRegexSpanKind.EnumMemberSynonymRegex] = new(Brightness: 0.45, BrightnessRange: 0.3),
        [TokenRegexSpanKind.EnumMemberJoiner] = new(Saturation: 0.12, Brightness: 0.5, SaturationRange: 0.2, BrightnessRange: 0.2),
        [TokenRegexSpanKind.EnumMemberName] = new(),
        [TokenRegexSpanKind.EnumMemberOccurrenceCount] = new(),
        [TokenRegexSpanKind.EnumMemberSynonymHeader] = new(),
        [TokenRegexSpanKind.EnumMemberSynonymFooter] = new(Saturation: 0.3, Brightness: 0.4, SaturationRange: 0.1, BrightnessRange: 0.1),
        [TokenRegexSpanKind.OmittedCount] = new(Saturation: 0, Brightness: 0.4, SaturationRange: 0),
        [TokenRegexSpanKind.ConnectiveSpace] = new(Saturation: 0, Brightness: 0.25, SaturationRange: 0),
        [TokenRegexSpanKind.LiteralMatch] = new(),
        [TokenRegexSpanKind.RegexJoiner] = new(Saturation: 0.18, Brightness: 0.5, SaturationRange: 0.2, BrightnessRange: 0.2),
        [TokenRegexSpanKind.GroupOpenHeaderText] = new(),
        [TokenRegexSpanKind.GroupOpenHeaderDisambiguator] = new(Saturation: 0.35, Brightness: 0.4, SaturationRange: 0.3, BrightnessRange: 0.25),
        [TokenRegexSpanKind.GroupFooterText] = new(),
        [TokenRegexSpanKind.GroupFooterQuantifierReminder] = new(Saturation: 0.35, Brightness: 0.4, SaturationRange: 0.3, BrightnessRange: 0.25),
        [TokenRegexSpanKind.GroupBorderWall] = new(),
        [TokenRegexSpanKind.RegexCommentSeparator] = new(Saturation: 0.15, Brightness: 0.25, SaturationRange: 0.2, BrightnessRange: 0.2),
    };

    /// <summary>
    /// Per-(role, NodeType) overrides, checked before <see cref="_roleSpecs"/>. Only saturation/brightness
    /// vary here — a border's color always identifies its own container, never its kind — bolder for an
    /// Enum's solid box, more muted for a TokenUnit's dashed one.
    /// </summary>
    static readonly Dictionary<(TokenRegexSpanKind Kind, CaptureNodeType NodeType), ColorKnobs> _roleNodeTypeSpecs = new()
    {
        [(TokenRegexSpanKind.GroupBorderWall, CaptureNodeType.Enum)] = new(Saturation: 0.75, Brightness: 0.6),
        [(TokenRegexSpanKind.GroupBorderWall, CaptureNodeType.TokenUnit)] = new(Saturation: 0.35, Brightness: 0.45),
    };

    /// <summary>
    /// The color treatment for <paramref name="kind"/>, refined by <paramref name="nodeType"/> when a more
    /// specific entry exists. <paramref name="positionalHueDegrees"/> is always used as this span's hue —
    /// the resolved spec's own <see cref="ColorKnobs.HueDegrees"/> is a placeholder and gets overwritten.
    /// When <paramref name="forceGrayscale"/> is set (the enclosing group itself has no hue — e.g. the
    /// transparent root), saturation is pinned to zero so role-tagged spans render neutral instead of
    /// reinterpreting the group's undefined hue as red.
    /// </summary>
    public static SpanColorPalette Resolve(TokenRegexSpanKind kind, double positionalHueDegrees, CaptureNodeType? nodeType = null, bool forceGrayscale = false)
    {
        var spec = nodeType is { } concreteNodeType && _roleNodeTypeSpecs.TryGetValue((kind, concreteNodeType), out var nodeTypeSpec)
            ? nodeTypeSpec
            : _roleSpecs[kind];

        var knobs = spec with
        {
            HueDegrees = positionalHueDegrees,
            Saturation = forceGrayscale ? 0 : spec.Saturation,
            SaturationRange = forceGrayscale ? 0 : spec.SaturationRange,
        };

        return SpanColorPalette.FromKnobs(knobs);
    }
}
