using Glyphotype.Distiller.Workbench;

namespace DocumentAnalysisInterface.Agent;

/// <summary>
/// An agent step's figures - what it spent (see <see cref="StepEffort"/>) and what it did to the score (see
/// <see cref="StepScoreChange"/>) - as the chat pane and the working changes show them: a share of them, for one of the
/// changes a step began, or several steps' added up, for a round.
/// </summary>
public sealed record StepFigures(TimeSpan? Elapsed, long? Tokens, double? CostUsd, double? Bits, double? Coverage)
{
    public static StepFigures Of(WorkbenchStep step) =>
        new(step.Effort?.Elapsed, step.Effort?.Tokens, step.Effort?.CostUsd, step.ScoreChange?.Bits, step.ScoreChange?.Coverage);

    /// <summary>Several steps' figures added up - each figure null only where none of the steps has it.</summary>
    public static StepFigures Sum(IEnumerable<WorkbenchStep> steps)
    {
        var figures = steps.Select(Of).ToList();

        T? Add<T>(Func<StepFigures, T?> figure, Func<T, T, T> add) where T : struct =>
            figures.Select(figure).Where(x => x is not null).Aggregate((T?)null, (sum, x) => sum is T total ? add(total, x.Value) : x);

        return new(Add(x => x.Elapsed, (a, b) => a + b), Add(x => x.Tokens, (a, b) => a + b), Add(x => x.CostUsd, (a, b) => a + b),
            Add(x => x.Bits, (a, b) => a + b), Add(x => x.Coverage, (a, b) => a + b));
    }

    /// <summary>One of <paramref name="shares"/> equal shares of these figures.</summary>
    public StepFigures Share(int shares) => shares <= 1 ? this : new(Elapsed / shares, Tokens / shares, CostUsd / shares, Bits / shares, Coverage / shares);

    public bool HasEffort => Elapsed is not null;

    public string ElapsedText => Elapsed is TimeSpan elapsed ? Duration(elapsed) : "";

    public string TokensText => Tokens is long tokens ? Compact(tokens) : "";

    public string CostText => CostUsd is double cost ? "~" + cost.ToString(cost < 1 ? "C2" : "C1") : HasEffort ? "?" : "";

    public string BitsText => Bits is double bits ? Signed(bits, "N0") : "";

    /// <summary>The coverage change in percentage points.</summary>
    public string CoverageText => Coverage is double coverage ? Signed(coverage * 100, "0.00") + "%" : "";

    /// <summary>Fewer bits is better; more coverage is.</summary>
    public string BitsClass => Bits is double bits ? Class(-bits) : "";

    public string CoverageClass => Coverage is double coverage ? Class(coverage) : "";

    /// <summary>The figures in a line, for the chat's rule under a step: those there are, joined by dots.</summary>
    public string Line() =>
        string.Join(" · ", new[]
        {
            ElapsedText,
            Tokens is null ? "" : TokensText + " tokens",
            CostText == "?" ? "" : CostText,
            Bits is null ? "" : BitsText + " bits",
            Coverage is null ? "" : CoverageText + " coverage",
        }.Where(x => x.Length > 0));

    static string Class(double gain) => Math.Abs(gain) < 1e-9 ? "unchanged" : gain > 0 ? "better" : "worse";

    static string Signed(double value, string format)
    {
        var text = Math.Abs(value).ToString(format);
        return text.Trim('0', '.', ',') == "" ? text : (value < 0 ? "−" : "+") + text;
    }

    /// <summary>A time worked in a few characters: 42s, 3m 05s, 1h 12m.</summary>
    public static string Duration(TimeSpan elapsed) =>
        elapsed.TotalHours >= 1 ? $"{(int)elapsed.TotalHours}h {elapsed.Minutes:00}m"
        : elapsed.TotalMinutes >= 1 ? $"{(int)elapsed.TotalMinutes}m {elapsed.Seconds:00}s"
        : $"{Math.Max(0, (int)Math.Round(elapsed.TotalSeconds))}s";

    /// <summary>A count of tokens in a few characters: 950, 24.1k, 1.2M.</summary>
    public static string Compact(long tokens) => tokens switch
    {
        < 1_000 => tokens.ToString(),
        < 100_000 => (tokens / 1_000.0).ToString("0.#") + "k",
        < 1_000_000 => (tokens / 1_000.0).ToString("0") + "k",
        _ => (tokens / 1_000_000.0).ToString("0.#") + "M",
    };
}
