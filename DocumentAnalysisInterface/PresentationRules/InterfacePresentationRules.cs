// The single home for every presentation knob that only the Blazor UI reads — theme colors, document
// text colors, echo underline treatment, the depth-to-pixel scales behind CorpusCapturesPage's nested
// underlines, hover behavior, and coverage formatting. Consolidated here from ThemeColor.cs,
// CoverageDisplay.cs, PresentationRules/HoverTreatmentConfig.cs, Glyphotype's
// RegexGeneration/Presentation/EchoPresentation.cs and DocumentTextPresentation.cs, and a scattering of
// inline magic numbers in EchoUnderline.razor / SpanView.razor / DocumentBlock.razor.
//
// This is one of exactly two knob files in the solution. The other is
// Glyphotype/PresentationRules/CorePresentationRules.cs, which holds the knobs Glyphotype's own render
// pipeline consumes (formatted regex layout/color, the C# class view, rainbow palette bases). The split
// is forced by the project reference direction: DocumentAnalysisInterface references Glyphotype, never
// the reverse, so a knob that SmartLineRenderer / GlyphClassRenderer / DeterministicPalette reads cannot
// live here. Everything else — anything whose only destination is a .razor file, an inline style, or a
// CSS custom property — belongs here rather than in the core library.
//
// Nothing outside these two files should hardcode a layout, color, font, or spacing constant.

using Glyphotype.PresentationRules;
using Glyphotype.RegexGeneration.Presentation;

namespace DocumentAnalysisInterface.PresentationRules;

/// <summary>
/// The named CSS custom properties defined in site.css, surfaced as C# constants so a component can
/// pass a theme color to a parameter (e.g. <c>IntInput.Color</c>) without restating the raw
/// <c>var(--...)</c> string. The colors themselves are defined in CSS; this is only the name list.
/// </summary>
public static class ThemeColor
{
    public const string DarkGrey = "var(--dark-grey)";
    public const string LightGrey = "var(--light-grey)";
    public const string SubtextGrey = "var(--subtext-grey)";
    public const string OutlineGrey = "var(--outline-grey)";
    public const string Navy = "var(--navy)";
    public const string FadedBlue = "var(--faded-blue)";
    public const string LightestBlue = "var(--lightest-blue)";
    public const string DarkTeal = "var(--dark-teal)";
    public const string DarkGold = "var(--dark-gold)";
}

/// <summary>
/// Static display tuning for CorpusCapturesPage's own body text color — captured text and
/// unmatched text independently, both distinct from the per-capture rainbow underline color in
/// <see cref="Glyphotype.Colors.HexPalette"/>. Not user-configurable at runtime; tune here and redeploy.
/// Both default to the app's ordinary body text color (site.css's <c>body { color }</c>), so
/// leaving these untouched changes nothing. Reach the page as the <c>--captured-text-color</c> /
/// <c>--unmatched-text-color</c> custom properties DocumentBlock sets once per line.
/// </summary>
public static class DocumentTextPresentation
{
    public const string CapturedTextColorHex = "#d4d4d4";
    public const string UnmatchedTextColorHex = "#989e9e";

    /// <summary>The thin rule drawn above each of a line's clauses after its first.</summary>
    public const string ClauseRuleColorHex = "#444444";

    /// <summary>Space in px kept clear above a clause rule.</summary>
    public const int ClauseRuleSpaceAbovePx = 10;

    /// <summary>Space in px kept clear below a clause rule - a little more than above, leaving room for the overlines drawn over terminal captures in the clause's first row.</summary>
    public const int ClauseRuleSpaceBelowPx = 14;
}

/// <summary>
/// Layout knobs for CorpusCapturesPage's content column — the vertical stack of <c>DocumentBlock</c>s
/// inside <c>.corpus-captures-container</c>. Not user-configurable at runtime; tune here and redeploy.
/// Reaches the page as CSS custom properties the page sets on that container.
/// </summary>
public static class CorpusCapturesLayout
{
    /// <summary>
    /// Maximum width in px of the content column. The container is centered (<c>margin-inline: auto</c>)
    /// in whatever horizontal space is left, so narrowing this just tightens the measure without
    /// shifting it off-center. Surfaces as the <c>--corpus-captures-max-width</c> custom property.
    /// </summary>
    public const int ContentMaxWidthPx = 1920;
}

/// <summary>
/// The depth-to-pixel scales behind CorpusCapturesPage's stacked underlines. Capture underlines
/// (SpanView) and echo underlines (EchoUnderline) deliberately share one scale so an echo at lane N
/// sits at exactly the same vertical offset as a capture at effective depth N — which is also what
/// lets <c>CaptureTraceDisplayContext</c> reserve a line's vertical space from a single
/// <c>MaxEffectiveDepth</c> covering both. Changing the step here moves both together, as it should.
/// </summary>
public static class CorpusCapturesFormatting
{
    /// <summary>Gap in px between the text baseline and the first (depth/lane 0) underline.</summary>
    public const int UnderlineBasePaddingPx = 2;

    /// <summary>Additional px of clearance per level of underline depth (or echo lane).</summary>
    public const int UnderlineDepthStepPx = 6;

    /// <summary>The document text's own font size in px, used to compute how much of a line's extra leading to crop off the top.</summary>
    public const int BodyFontSizePx = 16;

    /// <summary>Line height in px for a line with no nesting at all — enough for the text plus the depth-0 underline.</summary>
    public const int BaseLineHeightPx = 24;

    /// <summary>
    /// Additional line height in px per level of visible nesting. Deliberately larger than
    /// <see cref="UnderlineDepthStepPx"/>: the underlines themselves stack at the tighter step, and the
    /// extra slack here keeps the deepest one from crowding the following line's text.
    /// </summary>
    public const int LineHeightPerDepthPx = 8;

    /// <summary>The <c>padding-bottom</c> that places an underline at <paramref name="depth"/> (a capture's effective depth, or an echo's lane index — the same scale).</summary>
    public static int GetUnderlinePaddingPx(int depth) =>
        UnderlineBasePaddingPx + (depth * UnderlineDepthStepPx);

    /// <summary>Line height in px for a line whose deepest visible capture nesting / echo lane stack is <paramref name="maxEffectiveDepth"/>.</summary>
    public static int GetLineHeightPx(int maxEffectiveDepth) =>
        BaseLineHeightPx + (maxEffectiveDepth * LineHeightPerDepthPx);

    /// <summary>
    /// The negative top margin that crops away only the top half of the extra leading
    /// <see cref="GetLineHeightPx"/> introduces, so the reserved space all lands below the text where
    /// the underlines actually are.
    /// </summary>
    public static double GetTopCropPx(int maxEffectiveDepth) =>
        (GetLineHeightPx(maxEffectiveDepth) - BodyFontSizePx) / 2.0;
}

/// <summary>
/// Static display tuning for Echoes — the corpus-shared-subspan underlines CorpusCapturesPage draws
/// under UnmatchedString spans (see SpanView.GetEchoContainerStyle and EchoUnderline.GetLaneStyle).
/// Not user-configurable at runtime; tune here and redeploy. Lane offsets are not here — they come
/// off the shared <see cref="CorpusCapturesFormatting"/> scale, since an echo lane and a capture depth
/// have to land on the same rows.
/// </summary>
public static class EchoPresentation
{
    /// <summary>The underline color — one static value, not theme- or rank-varying.</summary>
    public const string UnderlineColorHex = "#777d7d";

    /// <summary>
    /// Brightness multiplier applied to <see cref="UnderlineColorHex"/> for an underline's own
    /// border while its row is hovered (e.g. 1.5 = 50% brighter). Independent of
    /// <see cref="TextHoverBrightnessFactor"/> — the underline and the word text it covers are
    /// brightened off two different base colors (this one, and <see cref="DocumentTextPresentation.UnmatchedTextColorHex"/>
    /// respectively), so they need their own factor to land looking equally "highlighted".
    /// </summary>
    public const double UnderlineHoverBrightnessFactor = 1.45;

    /// <summary>
    /// Brightness multiplier applied to <see cref="DocumentTextPresentation.UnmatchedTextColorHex"/>
    /// for the word text an echo covers while its underline is hovered. Kept separate from
    /// <see cref="UnderlineHoverBrightnessFactor"/>: the underline starts from a dim gray with a lot
    /// of headroom to brighten, while unmatched text is already fairly light, so the same factor
    /// would either barely move the text or blow the underline out.
    /// </summary>
    public const double TextHoverBrightnessFactor = 2;

    /// <summary>
    /// CSS <c>brightness()</c> multiplier applied to a count badge relative to <see cref="UnderlineColorHex"/>
    /// — kept as a factor on the same base color, rather than a second hardcoded hex, so the badge
    /// always reads as "the underline's color, lighter" even if the underline color changes.
    /// </summary>
    public const double BadgeBrightnessFactor = 1.6;

    /// <summary>
    /// <see cref="UnderlineColorHex"/> pre-brightened by <see cref="UnderlineHoverBrightnessFactor"/>,
    /// applied as an actual <c>border-bottom-color</c> on hover rather than a CSS
    /// <c>filter: brightness()</c>. A filter isn't safe here: it brightens an element's whole
    /// rendered subtree, and echo lanes at one word are nested one inside another (so several can
    /// wrap the same text) — a filter on the hovered lane would visually brighten whichever
    /// unrelated lanes happen to be nested inside or around it too, not just its own border.
    /// </summary>
    public static readonly string UnderlineHoverColorHex = ApplyBrightness(UnderlineColorHex, UnderlineHoverBrightnessFactor);

    /// <summary>
    /// <see cref="DocumentTextPresentation.UnmatchedTextColorHex"/> pre-brightened by
    /// <see cref="TextHoverBrightnessFactor"/>, applied as the word text's own <c>color</c> on
    /// hover — same direct-property approach as <see cref="UnderlineHoverColorHex"/> and for the
    /// same reason (no <c>filter</c>, no subtree bleed).
    /// </summary>
    public static readonly string TextHoverColorHex = ApplyBrightness(DocumentTextPresentation.UnmatchedTextColorHex, TextHoverBrightnessFactor);

    static string ApplyBrightness(string hex, double factor)
    {
        hex = hex.TrimStart('#');
        int r = Convert.ToInt32(hex.Substring(0, 2), 16);
        int g = Convert.ToInt32(hex.Substring(2, 2), 16);
        int b = Convert.ToInt32(hex.Substring(4, 2), 16);

        r = Math.Clamp((int)(r * factor), 0, 255);
        g = Math.Clamp((int)(g * factor), 0, 255);
        b = Math.Clamp((int)(b * factor), 0, 255);

        return $"#{r:x2}{g:x2}{b:x2}";
    }
}

/// <summary>
/// The single, centralized set of tuning knobs for hover-driven color treatment on any element
/// rendered from a positional rainbow palette (<see cref="Glyphotype.Colors.ColorKnobs"/> /
/// <see cref="Glyphotype.Colors.HexPalette"/>) — currently the type-expressions page's regex
/// spans and type-tree boxes (see glyph-regex.ts), and meant to be reused as-is by any future
/// element of the same kind (e.g. a CorpusCaptures underline) rather than each inventing its own
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

/// <summary>
/// Static display tuning for the regex-debug workflow on the Corpus Captures page: the custom color the
/// left-click-drag text highlight renders in (reaching the browser as the
/// <c>--regex-debug-selection-bg</c> custom property regex-debug.js sets from these values), and the
/// grey treatment the Debug Regex dialog paints over the non-matching tail of an "Original Regex"
/// preview. Not user-configurable at runtime; tune here and redeploy. Lives on this side of the knob-file
/// split (not <see cref="Glyphotype.PresentationRules"/>) because only the Blazor UI and its JS ever read
/// these — nothing in Glyphotype's own render pipeline does.
/// </summary>
public static class RegexDebugPresentation
{
    /// <summary>
    /// The selection highlight's color — a warm amber, deliberately off every hue family already in play
    /// on the page (the rainbow capture underlines, the grey echo underlines), so a debug selection reads
    /// as its own kind of mark rather than another capture.
    /// </summary>
    public const string SelectionHighlightColorHex = "#ffb454";

    /// <summary>The selection highlight's background transparency (0..1) — low enough that the rainbow underlines stay legible through it.</summary>
    public const double SelectionHighlightOpacity = 0.30;

    /// <summary>
    /// The single grey the dialog's "Original Regex" preview substitutes for every rainbow palette on
    /// lines at/after the first failure — the not-reached portion recedes while the matched portion keeps
    /// its normal coloring. One flat trio (rest/hi/lo all similar) since greyed lines shouldn't respond
    /// to hover emphasis the way live ones do.
    /// </summary>
    public static readonly Glyphotype.Colors.SpanStylePalette UnmatchedRegexGreyPalette = new("#5c6262", "#7a8282", "#454a4a");
}

/// <summary>
/// Shared formatting for capture-coverage percentages shown across the Corpus Captures page:
/// whole-number at the 0%/100% extremes (no visual noise implying more precision than "none"/"all"
/// actually carries), two decimals everywhere in between.
/// </summary>
public static class CoverageDisplay
{
    public static string FormatPercent(double percent) =>
        percent <= 0 ? "0%" :
        percent >= 100 ? "100%" :
        $"{percent:0.00}%";

    public static string GetColorClass(double percent) =>
        percent >= 100 ? "coverage-full" :
        percent <= 0 ? "coverage-none" :
        "coverage-partial";
}

/// <summary>How an <see cref="Glyphotype.GlyphPrimitives.BackReference"/>'s resolution reads on the Corpus Captures page.</summary>
public static class BackReferenceDisplay
{
    /// <summary>
    /// Where <paramref name="backReference"/> was resolved to, as a property path ("Chooser.Player"), each part paired
    /// with the capture it names - a dynamic property's resolved glyph, which has none of its own, the property's - and
    /// the type of what it names. The path is the shortest ending of the referent's full one, at least two parts
    /// long, that no other capture on the line also ends with: "Chosen.CardType", not
    /// "Effect.MayChooseAndPayForEach.Chosen.CardType", unless something else on the line ends in "Chosen.CardType" too.
    /// A single part, with no capture, for <c>{this}</c> (the document itself, a <see cref="Glyphotype.BackReferences.This"/>),
    /// or "unresolved" when nothing before it agreed with it.
    /// </summary>
    public static IReadOnlyList<(string Text, Glyphotype.RegexGeneration.Graph.CaptureTrace Capture, Type Type)> ReferentPath(
        Glyphotype.GlyphPrimitives.BackReference backReference,
        IEnumerable<Glyphotype.RegexGeneration.Graph.RootCaptureTrace> lineRoots)
    {
        if (backReference.Antecedent is not { } antecedent)
            return [("unresolved", null, null)];

        if (antecedent.IsSelf)
            return [(Glyphotype.Interfaces.IDocument.ThisToken, null, typeof(Glyphotype.BackReferences.This))];

        var path = antecedent.Trace.FullyQualifiedName.Split('_');
        var lineNames = lineRoots.Where(x => !x.IsSynthesized).SelectMany(x => x.GetFlatCaptureTree().Keys).Select(x => x.Split('_')).ToList();

        var length = Enumerable.Range(Math.Min(2, path.Length), path.Length - Math.Min(2, path.Length) + 1)
            .FirstOrDefault(n => lineNames.Count(name => name.Length >= n && name[^n..].SequenceEqual(path[^n..])) == 1, path.Length);

        var root = antecedent.Trace.CaptureContext?.RootCaptureTrace;
        var parts = new List<(string, Glyphotype.RegexGeneration.Graph.CaptureTrace, Type)>();
        Glyphotype.RegexGeneration.Graph.CaptureTrace capture = null;

        // Walked from the root, not just over the parts shown: a part with no capture of its own is the type the one
        // before it resolved to, and that one may be trimmed off the front.
        for (var i = 0; i < path.Length; i++)
        {
            capture = i == path.Length - 1 ? antecedent.Trace : root?[string.Join('_', path[..(i + 1)])] ?? capture;

            if (i >= path.Length - length)
                parts.Add((path[i], capture, capture?.ResolvedNodeType));
        }

        return parts;
    }

    /// <summary>
    /// Whether <paramref name="trace"/> is a back-reference with nothing captured inside it (a bare "it"): not
    /// <see cref="Glyphotype.RegexGeneration.Graph.CaptureTrace.IsTerminal"/>, being a Glyph, but with no children of its
    /// own - so it's shown like a terminal: a row in its parent's table rather than a table of its own, and overlined
    /// in the line above.
    /// </summary>
    public static bool IsLeafBackReference(Glyphotype.RegexGeneration.Graph.CaptureTrace trace) =>
        trace.ClrValue is Glyphotype.GlyphPrimitives.BackReference && !trace.EffectiveChildren.Any();

    /// <summary>
    /// Whether <paramref name="leaf"/>, a row of <paramref name="branch"/>'s table, shows a referent: its own property is
    /// marked [Referent], or it's the alternative a [Referent] one-of resolved to.
    /// </summary>
    public static bool IsReferent(Glyphotype.RegexGeneration.Graph.CaptureTrace leaf, Glyphotype.RegexGeneration.Graph.CaptureTrace branch) =>
        IsMarked(leaf) || branch.ClrValue is Glyphotype.GlyphPrimitives.OneOfBase && IsMarked(branch);

    static bool IsMarked(Glyphotype.RegexGeneration.Graph.CaptureTrace trace) =>
        trace.SourceNode?.Navigation?.Prop?.IsDefined(typeof(Glyphotype.Attributes.ReferentAttribute), inherit: true) == true;
}
