namespace CardAnalysisInterface;

public static class Extensions
{
    public static string ToInlineStyle(this Dictionary<string, string> cssProperties) 
        => string.Join("; ", cssProperties.Select(x => x.Key + ": " + x.Value));

    public static string ToColorStyle(this DeterministicPalette palette, string additionalStyles = null, int shift = 0)
    {
        string style = shift switch
        {
            0 => $"--color: {palette.Hex}; --highlight-color: {palette.HexLight}; --lowlight-color: {palette.HexDark};",
            >= 1 => $"--color: {palette.HexLight}; --highlight-color: {palette.HexLight}; --lowlight-color: {palette.Hex};",
            <= 1 => $"--color: {palette.HexDark}; --highlight-color: {palette.Hex}; --lowlight-color: {palette.HexDark};"
        };

        if (!string.IsNullOrEmpty(additionalStyles))
            style += " " + additionalStyles;

        return style;
    }
}
