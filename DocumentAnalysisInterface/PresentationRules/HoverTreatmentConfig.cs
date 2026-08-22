using Glyphotype.RegexGeneration.Presentation;

namespace DocumentAnalysisInterface.PresentationRules;

/// <summary>
/// The single, centralized set of tuning knobs for hover-driven color treatment on any element
/// rendered from a positional rainbow palette (<see cref="Glyphotype.Colors.ColorKnobs"/> /
/// <see cref="Glyphotype.Colors.HexPalette"/>) — currently the type-expressions page's regex
/// spans and type-tree boxes (see type-regex.ts), and meant to be reused as-is by any future
/// element of the same kind (e.g. a DocumentLines underline) rather than each inventing its own
/// dim/fade/pop numbers. Covers: how dim an ancestor-of-target or unrelated element reads, how
/// long a hover must rest before its highlight state commits, how long the resulting fade
/// takes, and how the direct hover target briefly "pops" brighter before easing down to its
/// resting highlighted brightness. Passed to the JS side once via
/// <c>initializeTypeExpressionsHover</c> so these live here instead of being hardcoded in
/// TypeScript or duplicated per element type.
/// </summary>
public static class HoverTreatmentConfig
{
    /// <summary>
    /// Opacity applied to an element that's an ancestor of the hovered path — on the "path up
    /// to" the target, relevant but not the target itself — halfway between fully dimmed and
    /// full opacity by default.
    /// </summary>
    public const double PathAncestorDimOpacity = 0.5;

    /// <summary>
    /// Opacity applied to any element unrelated to the hovered path — shared by every element
    /// type's lowlight treatment. Each element type may *additionally* swap toward its own
    /// darker/desaturated palette variant to keep its own hue visible while dimmed (a regex
    /// span swaps to its precomputed Lo color, a type-tree box to its Dark variant — see
    /// <see cref="ColorKnobDefaults"/> and
    /// <see cref="Glyphotype.Colors.DeterministicPalette"/> respectively for how those colors
    /// themselves are computed), but the "how dim" opacity itself is this one shared value.
    /// </summary>
    public const double LowlightOpacity = 0.35;

    /// <summary>
    /// How long a hover must rest on a new target before its highlight state commits. Debounces
    /// the flicker that would otherwise result from the cursor rapidly entering/leaving spans as
    /// it travels across rows of them.
    /// </summary>
    public const int DebounceMs = 5;

    /// <summary>How long the color/opacity crossfade takes once a new highlight state commits.</summary>
    public const int FadeDurationMs = 150;

    /// <summary>
    /// How much brighter than its resting highlighted brightness the direct hover target briefly
    /// "pops" to, as a fraction (e.g. 0.35 means a peak 35% brighter than target), before easing
    /// back down. Applies only to the exact-match target, so it stands out from the ghostier
    /// path-ancestor and lingering-fade elements around it.
    /// </summary>
    public const double OvershootBrightnessBoost = 0.35;

    /// <summary>How long the pop takes to reach its peak brightness.</summary>
    public const int OvershootDurationMs = 40;

    /// <summary>How long the pop takes to ease back down from its peak to resting highlighted brightness.</summary>
    public const int OvershootSettleDurationMs = 1500;

    /// <summary>
    /// <see cref="RegexSpanKind"/>s that never participate in the hover highlight system at
    /// all — a span tagged with one of these gets no <c>data-path</c>, so hovering it does
    /// nothing: no highlight, no lowlight of everything else, as if it weren't there. Currently
    /// excludes the structural "glue" of a formatted regex (joiners and literal-match text, both
    /// their regex-column and comment-column renderings, plus the connective spaces between
    /// them) — these rarely carry meaning worth cross-referencing against the type tree, and
    /// constantly triggering highlight state while passing over them would be noise.
    /// </summary>
    public static readonly IReadOnlySet<RegexSpanKind> NonInteractiveSpanKinds = new HashSet<RegexSpanKind>
    {
        RegexSpanKind.RegexJoiner,
        RegexSpanKind.CommentJoiner,
        RegexSpanKind.RegexLiteralMatch,
        RegexSpanKind.CommentLiteralMatch,
        RegexSpanKind.RegexConnectiveSpace,
    };
}
