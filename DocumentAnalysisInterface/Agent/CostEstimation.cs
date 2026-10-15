using System.Text.Json;

namespace DocumentAnalysisInterface.Agent;

/// <summary>Tokens an agent used, by kind: each costs differently.</summary>
public sealed record TokenUsage(long Input, long CacheWrite, long CacheRead, long Output)
{
    public static TokenUsage None { get; } = new(0, 0, 0, 0);

    public long Total => Input + CacheWrite + CacheRead + Output;

    public static TokenUsage operator -(TokenUsage a, TokenUsage b) =>
        new(a.Input - b.Input, a.CacheWrite - b.CacheWrite, a.CacheRead - b.CacheRead, a.Output - b.Output);

    public static TokenUsage Of(ChatStats stats) => new(stats.InputTokens, stats.CacheWriteTokens, stats.CacheReadTokens, stats.OutputTokens);
}

/// <summary>
/// What an agent's work costs, in dollars - an estimate, always given. What matters most is that it's consistent, so steps
/// can be compared; the figure itself is a best guess.
/// </summary>
public interface ICostEstimator
{
    /// <summary>What <paramref name="usage"/> on <paramref name="model"/> (as the CLI names it) cost - and, where the account's weekly allowance is known, a sample to refine the estimate by.</summary>
    double Estimate(TokenUsage usage, string model, UsageWindow weeklyAllowance);
}

/// <summary>
/// Costs reckoned against a Claude subscription: the share of the subscription's weekly allowance the tokens used, of
/// what a week of the subscription costs. Every assumption is here, and only here.
/// <para>
/// The allowance is metered in something close to what the API would charge for the same tokens, so tokens are first
/// weighed at the API's list prices (by model, and by kind: a token read from the cache costs a tenth of one read
/// fresh, one written out several times one read in). How many API-dollars a week's allowance holds isn't published,
/// so it starts from a guess, and is measured as the allowance's reported use moves while the agent works: every
/// sample - API-dollars spent against the share of the allowance used meanwhile - pulls it toward what's observed. The
/// samples count whatever else used the allowance meanwhile, so if anything they overstate a cost. They're kept between
/// runs of the app.
/// </para>
/// </summary>
public sealed class SubscriptionCostEstimator(string statePath) : ICostEstimator
{
    /// <summary>The subscription: $20 a month, and so a week's share of it.</summary>
    const double _monthlyUsd = 20;
    const double _weeklyUsd = _monthlyUsd * 12 / 52;

    /// <summary>
    /// The guess at how many API-dollars a week's allowance holds, before anything's measured - and how much of a week's
    /// use the guess counts for against what's measured (a twentieth: after the allowance has moved a few points, what's
    /// observed mostly decides).
    /// </summary>
    const double _guessedWeeklyApiUsd = 75;
    const double _guessWeight = 0.05;

    /// <summary>The least the allowance's use must move for a sample to count: it's reported coarsely.</summary>
    const double _minimumSampleUtilization = 0.01;

    /// <summary>API list prices in dollars per million tokens - input, and output - by model family; a cache write costs 1.25 times an input token, a cache read a tenth of one.</summary>
    static readonly Dictionary<string, (double Input, double Output)> _prices = new()
    {
        ["haiku"] = (1, 5),
        ["sonnet"] = (3, 15),
        ["opus"] = (5, 25),

        // Not published when this was written: priced as the tier above Opus.
        ["fable"] = (15, 75),
    };

    const double _cacheWriteFactor = 1.25;
    const double _cacheReadFactor = 0.1;

    readonly object _gate = new();

    /// <summary>What's been measured, kept between runs: API-dollars spent against the share of the allowance used meanwhile, and the last reading of the allowance with what had been spent by then.</summary>
    sealed record State(double MeasuredApiUsd = 0, double MeasuredUtilization = 0, double? LastUtilization = null, DateTimeOffset? LastResetsAt = null, double SpentSinceLast = 0);

    State _state;

    public double Estimate(TokenUsage usage, string model, UsageWindow weeklyAllowance)
    {
        var apiUsd = ApiUsd(usage, model);

        lock (_gate)
        {
            _state ??= Load();
            Sample(apiUsd, weeklyAllowance);

            // The API-dollars a week's allowance holds: the guess, weighed against what's been measured.
            var weeklyApiUsd = (_guessedWeeklyApiUsd * _guessWeight + _state.MeasuredApiUsd) / (_guessWeight + _state.MeasuredUtilization);

            return _weeklyUsd * apiUsd / weeklyApiUsd;
        }
    }

    /// <summary>Adds what was just spent to the sample under way, and closes it once the allowance's use has moved enough to measure.</summary>
    void Sample(double apiUsd, UsageWindow weekly)
    {
        var spent = _state.SpentSinceLast + apiUsd;

        if (weekly is null)
        {
            _state = _state with { SpentSinceLast = spent };
            return;
        }

        // A first reading, or the allowance having started over: what was spent before it can't be set against anything.
        if (_state.LastUtilization is not double last || StartedOver(weekly.ResetsAt, _state.LastResetsAt) || weekly.Utilization < last)
            _state = _state with { LastUtilization = weekly.Utilization, LastResetsAt = weekly.ResetsAt, SpentSinceLast = 0 };
        else if (weekly.Utilization - last >= _minimumSampleUtilization)
            _state = _state with
            {
                MeasuredApiUsd = _state.MeasuredApiUsd + spent,
                MeasuredUtilization = _state.MeasuredUtilization + (weekly.Utilization - last),
                LastUtilization = weekly.Utilization,
                SpentSinceLast = 0,
            };
        else
            _state = _state with { SpentSinceLast = spent };

        Save();
    }

    /// <summary>Whether the allowance's reset has moved on - by more than its reported time wanders.</summary>
    static bool StartedOver(DateTimeOffset? resetsAt, DateTimeOffset? last) =>
        resetsAt is DateTimeOffset now && last is DateTimeOffset then && (now - then).Duration() > TimeSpan.FromHours(1);

    /// <summary>What <paramref name="usage"/> would cost at the API's list prices - a model not known by its family priced as Sonnet.</summary>
    static double ApiUsd(TokenUsage usage, string model)
    {
        var family = _prices.Keys.FirstOrDefault(x => model?.Contains(x, StringComparison.OrdinalIgnoreCase) == true) ?? "sonnet";
        var (input, output) = _prices[family];

        return (usage.Input * input
            + usage.CacheWrite * input * _cacheWriteFactor
            + usage.CacheRead * input * _cacheReadFactor
            + usage.Output * output) / 1_000_000;
    }

    State Load()
    {
        try
        {
            return File.Exists(statePath) ? JsonSerializer.Deserialize<State>(File.ReadAllText(statePath)) ?? new() : new();
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return new();
        }
    }

    void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(statePath));
            File.WriteAllText(statePath, JsonSerializer.Serialize(_state));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // What's measured is a refinement: the estimate holds without it.
        }
    }
}
