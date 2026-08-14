namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// The centralized "control panel" of color knobs for every <see cref="TokenRegexSpanKind"/>. This is the
/// single place to retune how TypeRegexPage colors any of these span roles; nothing outside this class
/// should hardcode one of their colors. Sits alongside <see cref="SmartRegexStaticRules"/> as the other
/// "knobs" class for formatted regex output — that one governs layout, this one governs color.
/// </summary>
/// <remarks>
/// A role's hue is always whatever the caller passes into <see cref="Resolve"/> as
/// <c>positionalHueDegrees</c> — in practice, the same rainbow hue
/// <see cref="DeterministicPalette.GetPositionalPaletteSet{T}"/> already assigned to the span's enclosing
/// named group. That's deliberate for every role, including <see cref="TokenRegexSpanKind.CommentGroupBorderWall"/>:
/// nested boxes read best when each box's wall is its own container-meaningful color, so a border's hue
/// should never be pinned regardless of which group it belongs to; only its saturation/brightness are
/// hand-tuned below, same as every other role, with no further distinction by the enclosing group's
/// <see cref="CaptureNodeKind"/> (box shape — solid vs. dashed — already carries that distinction; see
/// <see cref="SmartRegexStaticRules.NodeTypeToBoxCharSet"/>).
/// </remarks>
public static class SmartSpanControlPanel
{
    /// <summary>Per-role knobs. <see cref="ColorKnobs.HueDegrees"/> is ignored here — it's always overwritten with the span's positional rainbow hue in <see cref="Resolve"/> — so only saturation/brightness (and their hi/lo ranges) are hand-tuned per role.</summary>
    static readonly Dictionary<TokenRegexSpanKind, ColorKnobs> _roleSpecs = new()
    {
        // --- Regex column ---

        // |tap
        [TokenRegexSpanKind.RegexEnumMember] = new(Saturation: .75, Brightness: .75),

        // |tap
        // ^
        [TokenRegexSpanKind.RegexEnumMemberJoiner] = new(Saturation: 0, Brightness: 0.25, SaturationRange: 0, BrightnessRange: 0),

        // until[ ]end
        //      ^^^
        [TokenRegexSpanKind.RegexConnectiveSpace] = new(Saturation: 0, Brightness: 0.25, SaturationRange: 0),

        // until end of turn
        [TokenRegexSpanKind.RegexLiteralMatch] = new(Saturation: 0.3, Brightness: 0.45, SaturationRange: 0.2, BrightnessRange: 0.2),

        // [ ]
        [TokenRegexSpanKind.RegexJoiner] = new(Saturation: 0.15, Brightness: 0.3, SaturationRange: 0.2, BrightnessRange: 0.2),

        // --- Regex/comment separator ---

        // (?<CardKeyword>tap)  #  Card Keyword
        //                    ^^^^^
        [TokenRegexSpanKind.RegexCommentSeparator] = new(Saturation: 0, Brightness: 0.25, SaturationRange: 0.2, BrightnessRange: 0.2),

        // --- Comment column ---

        // Tap : 12
        // ^^^
        [TokenRegexSpanKind.CommentEnumMemberName] = new(Saturation: .75, Brightness: .75),

        // Tap : 12
        //       ^^
        [TokenRegexSpanKind.CommentEnumMemberOccurrenceCount] = new(Saturation: .3, Brightness: .35, IsBold: true),

        // ───────Power and Toughness─
        //        ^^^^^^^^^^^^^^^^^^^
        [TokenRegexSpanKind.CommentEnumMemberSynonymFooter] = new(Saturation: 0.3, Brightness: 0.4, SaturationRange: 0.1, BrightnessRange: 0.1, IsItalic: true),

        // 3 omitted
        [TokenRegexSpanKind.CommentOmittedCount] = new(Saturation: 0, Brightness: 0.4, SaturationRange: 0),

        // literal match
        [TokenRegexSpanKind.CommentLiteralMatch] = new(Saturation: 0.3, Brightness: 0.45, SaturationRange: 0.2, BrightnessRange: 0.2),

        // joiner Space
        [TokenRegexSpanKind.CommentJoiner] = new(Saturation: 0.15, Brightness: 0.3, SaturationRange: 0.2, BrightnessRange: 0.2),

        // Token Unit: Card Keyword
        // ^^^^^^^^^^
        [TokenRegexSpanKind.CommentGroupOpenHeaderText] = new(Brightness: .4, Saturation: .4),

        // Token Unit: Card Keyword
        //            ^^^^^^^^^^^^^
        [TokenRegexSpanKind.CommentGroupOpenHeaderDisambiguator] = new(Saturation: 0.35, Brightness: 0.4, SaturationRange: 0.3, BrightnessRange: 0.25),

        // Card Keyword (any number)
        // ^^^^^^^^^^^^
        [TokenRegexSpanKind.CommentGroupFooterText] = new(IsItalic: true, Brightness: .4, Saturation: .4),

        // Card Keyword (any number)
        //              ^^^^^^^^^^^^
        [TokenRegexSpanKind.CommentGroupFooterQuantifierReminder] = new(Saturation: 0.35, Brightness: 0.4, SaturationRange: 0.3, BrightnessRange: 0.25),

        // ┌── Card Keyword ──┐
        // ^                  ^
        [TokenRegexSpanKind.CommentGroupBorderWall] = new(Brightness: .35, Saturation: .35),
    };

    /// <summary>
    /// The color treatment for <paramref name="kind"/>. <paramref name="positionalHueDegrees"/> is always
    /// used as this span's hue — the resolved spec's own <see cref="ColorKnobs.HueDegrees"/> is a placeholder
    /// and gets overwritten. When <paramref name="forceGrayscale"/> is set (the enclosing group itself has no
    /// hue — e.g. the transparent root), saturation is pinned to zero so role-tagged spans render neutral
    /// instead of reinterpreting the group's undefined hue as red.
    /// </summary>
    public static SpanStylePalette Resolve(TokenRegexSpanKind kind, double positionalHueDegrees, bool forceGrayscale = false)
    {
        var spec = _roleSpecs[kind];

        var knobs = spec with
        {
            HueDegrees = positionalHueDegrees,
            Saturation = forceGrayscale ? 0 : spec.Saturation,
            SaturationRange = forceGrayscale ? 0 : spec.SaturationRange,
        };

        return SpanStylePalette.FromKnobs(knobs);
    }
}
