namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Static display tuning for Echoes — the corpus-shared-subspan underlines DocumentLinesPage draws
/// under UnmatchedString spans (see DocumentAnalysisInterface's SpanView.GetEchoData/GetEchoRowStyle).
/// Not user-configurable at runtime; tune here and redeploy.
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
