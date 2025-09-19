namespace CardAnalysisInterface;

public static class Extensions
{
    public static string ToInlineStyle(this Dictionary<string, string> cssProperties) 
        => string.Join("; ", cssProperties.Select(x => x.Key + ": " + x.Value));

    /// <summary>
    /// Generates a string of CSS custom properties.
    /// </summary>
    /// <param name="palette">The color palette to use.</param>
    /// <param name="useSaturatedHighlight">If true, the --highlight-color will be the vibrant HexSat. If false, it will be the lighter HexLight.</param>
    /// <param name="additionalStyles">Any additional literal styles to append.</param>
    /// <param name="shift">Shifts the base color between Hex, HexLight, and HexDark.</param>
    public static string ToColorStyle(this Palette palette, string additionalStyles = null, bool useSaturatedHighlight = false, int shift = 0)
    {
        if (palette == null)
        {
            return string.Empty;
        }

        // Determine which highlight color to use based on the new flag.
        string highlightColor = useSaturatedHighlight ? palette.HexSat : palette.HexLight;

        string style = shift switch
        {
            // --highlight-color now uses the value from our variable above.
            0 => $"--color: {palette.Hex}; --highlight-color: {highlightColor}; --lowlight-color: {palette.HexDark};",
            >= 1 => $"--color: {palette.HexLight}; --highlight-color: {highlightColor}; --lowlight-color: {palette.Hex};",
            <= -1 => $"--color: {palette.HexDark}; --highlight-color: {highlightColor}; --lowlight-color: {palette.HexDark};"
        };

        if (!string.IsNullOrEmpty(additionalStyles))
            style += " " + additionalStyles;

        return style;
    }
}
