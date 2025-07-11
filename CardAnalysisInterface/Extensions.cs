namespace CardAnalysisInterface;

public static class Extensions
{
    public static string ToInlineStyle(this Dictionary<string, string> cssProperties) 
        => string.Join("; ", cssProperties.Select(x => x.Key + ": " + x.Value));

    public static string ToColorStyle(this DeterministicColorPalette palette, int shift = 0)
    {
        var defaultValues = $"--color: {palette.Hex}; --highlight-color: {palette.HexLight}; --lowlight-color: {palette.HexDark};";

        if (shift == 0)
            return defaultValues;

        if (shift >= 1)
            return $"--color: {palette.HexLight}; --highlight-color: {palette.HexLight}; --lowlight-color: {palette.Hex};";

        if (shift <= 1)
            return $"--color: {palette.HexDark}; --highlight-color: {palette.Hex}; --lowlight-color: {palette.HexDark};";

        return defaultValues;
    }
}
