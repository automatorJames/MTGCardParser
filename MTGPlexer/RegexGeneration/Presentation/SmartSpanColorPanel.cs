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
/// Unless a row below declares its own <see cref="SpanColorSpec.HueDegrees"/>, a role's hue is whatever the
/// caller passes into <see cref="Resolve"/> as <c>positionalHueDegrees</c> — in practice, the same rainbow
/// hue <see cref="DeterministicPalette.GetPositionalPaletteSet{T}"/> already assigned to the span's enclosing
/// named group. That's deliberate for every role, including <see cref="TokenRegexSpanKind.GroupBorderWall"/>:
/// nested boxes read best when each box's wall is its own container-meaningful color, so a border's hue
/// should never be pinned regardless of which group it belongs to. Its per-<see cref="CaptureNodeType"/>
/// entries below only vary saturation/brightness (bolder for an Enum's solid box, more muted for a
/// TokenUnit's dashed one) — the hue is still always the container's own.
/// </remarks>
public static class SmartSpanColorPanel
{
    /// <summary>Per-role knobs. None of these declare a hue, so all of them take on their span's positional rainbow hue; only saturation/brightness (and their hi/lo ranges) are hand-tuned per role.</summary>
    static readonly Dictionary<TokenRegexSpanKind, SpanColorSpec> _roleSpecs = new()
    {
        [TokenRegexSpanKind.EnumMemberRegex] = new(),
        [TokenRegexSpanKind.EnumMemberSynonymRegex] = new(Brightness: 0.45, BrightnessRange: 0.3),
        [TokenRegexSpanKind.EnumMemberJoiner] = new(Saturation: 0.12, Brightness: 0.5, SaturationRange: 0.2, BrightnessRange: 0.2),
        [TokenRegexSpanKind.EnumMemberName] = new(),
        [TokenRegexSpanKind.EnumMemberOccurrenceCount] = new(),
        [TokenRegexSpanKind.EnumMemberSynonymHeader] = new(),
        [TokenRegexSpanKind.EnumMemberSynonymFooter] = new(Saturation: 0.3, Brightness: 0.4, SaturationRange: 0.1, BrightnessRange: 0.1),
        [TokenRegexSpanKind.OmittedCount] = new(Saturation: 0, Brightness: 0.4, SaturationRange: 0),
        [TokenRegexSpanKind.ConnectiveSpace] = new(Saturation: 0, Brightness: 0.5, SaturationRange: 0),
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
    /// Per-(role, NodeType) overrides, checked before <see cref="_roleSpecs"/>. Neither entry declares a hue
    /// — a border's color always identifies its own container, never its kind — only saturation/brightness
    /// vary: bolder for an Enum's solid box, more muted for a TokenUnit's dashed one.
    /// </summary>
    static readonly Dictionary<(TokenRegexSpanKind Kind, CaptureNodeType NodeType), SpanColorSpec> _roleNodeTypeSpecs = new()
    {
        [(TokenRegexSpanKind.GroupBorderWall, CaptureNodeType.Enum)] = new(Saturation: 0.75, Brightness: 0.6),
        [(TokenRegexSpanKind.GroupBorderWall, CaptureNodeType.TokenUnit)] = new(Saturation: 0.35, Brightness: 0.45),
    };

    /// <summary>
    /// The color treatment for <paramref name="kind"/>, refined by <paramref name="nodeType"/> when a more
    /// specific entry exists. <paramref name="positionalHueDegrees"/> is used as this span's hue unless the
    /// resolved spec declares a fixed <see cref="SpanColorSpec.HueDegrees"/> of its own.
    /// </summary>
    public static SpanColorPalette Resolve(TokenRegexSpanKind kind, double positionalHueDegrees, CaptureNodeType? nodeType = null)
    {
        var spec = nodeType is { } concreteNodeType && _roleNodeTypeSpecs.TryGetValue((kind, concreteNodeType), out var nodeTypeSpec)
            ? nodeTypeSpec
            : _roleSpecs[kind];

        var knobs = new ColorKnobs(
            HueDegrees: spec.HueDegrees ?? positionalHueDegrees,
            Saturation: spec.Saturation,
            Brightness: spec.Brightness,
            SaturationRange: spec.SaturationRange,
            BrightnessRange: spec.BrightnessRange);

        return SpanColorPalette.FromKnobs(knobs);
    }
}

/// <summary>Author-facing row of <see cref="SmartSpanColorPanel"/>'s tables: like <see cref="ColorKnobs"/>, but <see cref="HueDegrees"/> may be omitted to fall back to the span's own positional rainbow hue instead of hand-picking one.</summary>
readonly record struct SpanColorSpec(
    double? HueDegrees = null,
    double? Saturation = null,
    double? Brightness = null,
    double? SaturationRange = null,
    double? BrightnessRange = null);
