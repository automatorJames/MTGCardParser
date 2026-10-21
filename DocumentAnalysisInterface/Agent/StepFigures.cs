using Glyphotype.Distiller.Workbench;

namespace DocumentAnalysisInterface.Agent;

/// <summary>
/// An agent step's figures - what it spent (see <see cref="StepEffort"/>) and what it did to the score (see
/// <see cref="StepScoreChange"/>) - as the chat pane and the working changes show them: one step's, or a round's steps'
/// added up.
/// </summary>
/// <param name="Bits">The change in total bits.</param>
/// <param name="Compression">The change in total bits as a share of the no-grammar baseline.</param>
/// <param name="Words">The change in words covered.</param>
/// <param name="Coverage">The change in coverage, as a share of the corpus's words.</param>
public sealed record StepFigures(TimeSpan? Elapsed, long? Tokens, double? CostUsd, double? Bits, double? Compression, int? Words, double? Coverage)
{
    public static StepFigures Of(WorkbenchStep step) =>
        new(step.Effort?.Elapsed, step.Effort?.Tokens, step.Effort?.CostUsd,
            step.ScoreChange?.Bits, step.ScoreChange?.Compression, step.ScoreChange?.Words, step.ScoreChange?.Coverage);

    /// <summary>Several steps' figures added up - each figure null only where none of the steps has it.</summary>
    public static StepFigures Sum(IEnumerable<WorkbenchStep> steps)
    {
        var figures = steps.Select(Of).ToList();

        T? Add<T>(Func<StepFigures, T?> figure, Func<T, T, T> add) where T : struct =>
            figures.Select(figure).Where(x => x is not null).Aggregate((T?)null, (sum, x) => sum is T total ? add(total, x.Value) : x);

        return new(Add(x => x.Elapsed, (a, b) => a + b), Add(x => x.Tokens, (a, b) => a + b), Add(x => x.CostUsd, (a, b) => a + b),
            Add(x => x.Bits, (a, b) => a + b), Add(x => x.Compression, (a, b) => a + b), Add(x => x.Words, (a, b) => a + b), Add(x => x.Coverage, (a, b) => a + b));
    }

    public string ElapsedText => Elapsed is TimeSpan elapsed ? Duration(elapsed) : "";

    public string TokensText => Tokens is long tokens ? Compact(tokens) : "";

    /// <summary>The estimated cost - to a tenth of a cent under a dime, where cents would round most steps to nothing.</summary>
    public string CostText => CostUsd is double cost ? "~" + cost.ToString(cost < 0.1 ? "$0.000" : cost < 1 ? "C2" : "C1") : "";

    public string BitsText => Bits is double bits ? Signed(bits, "N0") : "";

    /// <summary>The change in total bits, in percentage points of the baseline.</summary>
    public string CompressionText => Compression is double compression ? Signed(compression * 100, "0.00") + "%" : "";

    public string WordsText => Words is int words ? Signed(words, "N0") : "";

    /// <summary>The change in coverage, in percentage points.</summary>
    public string CoverageText => Coverage is double coverage ? Signed(coverage * 100, "0.00") + "%" : "";

    /// <summary>Fewer bits is better; more coverage is.</summary>
    public string BitsClass => Bits is double bits ? Class(-bits) : "";

    public string CompressionClass => Compression is double compression ? Class(-compression) : "";

    public string WordsClass => Words is int words ? Class(words) : "";

    public string CoverageClass => Coverage is double coverage ? Class(coverage) : "";

    /// <summary>The figures in a line, as text: those there are, joined by dots.</summary>
    public string Line() => string.Join(" · ", Pairs().Select(x => $"{x.Key} {x.Value}"));

    /// <summary>The figures there are, each with what it is, for the chat's summary under a step.</summary>
    public IEnumerable<(string Key, string Value)> Pairs()
    {
        if (Elapsed is not null)
            yield return ("Time", ElapsedText);

        if (Tokens is not null)
            yield return ("Tokens", TokensText);

        if (CostUsd is not null)
            yield return ("Cost", CostText);

        if (Bits is not null)
            yield return ("Bits", BitsText + (Compression is null ? "" : $" ({CompressionText})"));

        if (Coverage is not null)
            yield return ("Coverage", Words is null ? CoverageText : $"{WordsText} words ({CoverageText})");
    }

    static string Class(double gain) => Math.Abs(gain) < 1e-9 ? "unchanged" : gain > 0 ? "better" : "worse";

    static string Signed(double value, string format)
    {
        var text = Math.Abs(value).ToString(format);
        return text.Trim('0', '.', ',') == "" ? text : (value < 0 ? "−" : "+") + text;
    }

    /// <summary>A time worked in a few characters, to a tenth of a second under the minute: 6.7s, 10m 28s, 1h 12m.</summary>
    public static string Duration(TimeSpan elapsed) =>
        elapsed.TotalHours >= 1 ? $"{(int)elapsed.TotalHours}h {elapsed.Minutes:00}m"
        : elapsed.TotalMinutes >= 1 ? $"{(int)elapsed.TotalMinutes}m {elapsed.Seconds:00}s"
        : $"{Tenths(Math.Max(0, elapsed.TotalSeconds)):0.0}s";

    /// <summary>Seconds cut down to the tenth - not rounded, so 59.97 seconds never reads as 60.0.</summary>
    static double Tenths(double seconds) => Math.Floor(seconds * 10) / 10;

    /// <summary>A count of tokens in a few characters: 950, 24.1k, 1.2M.</summary>
    public static string Compact(long tokens) => tokens switch
    {
        < 1_000 => tokens.ToString(),
        < 100_000 => (tokens / 1_000.0).ToString("0.#") + "k",
        < 1_000_000 => (tokens / 1_000.0).ToString("0") + "k",
        _ => (tokens / 1_000_000.0).ToString("0.#") + "M",
    };
}
