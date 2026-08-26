namespace DocumentAnalysisInterface;

/// <summary>
/// Shared formatting for capture-coverage percentages shown across the Document Lines page:
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
