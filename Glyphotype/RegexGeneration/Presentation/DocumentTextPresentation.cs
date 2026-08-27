namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Static display tuning for DocumentLinesPage's own body text color — captured text and
/// unmatched text independently, both distinct from the per-capture rainbow underline color in
/// <see cref="Colors.HexPalette"/>. Not user-configurable at runtime; tune here and redeploy.
/// Both default to the app's ordinary body text color (site.css's <c>body { color }</c>), so
/// leaving these untouched changes nothing.
/// </summary>
public static class DocumentTextPresentation
{
    public const string CapturedTextColorHex = "#d4d4d4";
    public const string UnmatchedTextColorHex = "#989e9e";
}
