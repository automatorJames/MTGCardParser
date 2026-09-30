namespace Glyphotype.Distiller.Scoring;

/// <summary>Code lengths, in bits, of the codes the scorer describes grammars and corpora with.</summary>
public static class CodeLength
{
    static readonly List<double> _log2Factorials = [0];
    static readonly object _gate = new();

    /// <summary>A choice among <paramref name="alternatives"/> equally likely ones.</summary>
    public static double Uniform(long alternatives) =>
        alternatives <= 1 ? 0 : Math.Log2(alternatives);

    /// <summary>The Elias gamma code for <paramref name="n"/> ≥ 1: a universal code, costing about 2·log2(n) bits, for an integer with no known bound.</summary>
    public static double EliasGamma(long n) =>
        n >= 1 ? 2 * Math.Floor(Math.Log2(n)) + 1 : throw new ArgumentOutOfRangeException(nameof(n), n, "Elias gamma codes integers from 1");

    /// <summary>An unbounded count ≥ 0.</summary>
    public static double Count(long n) => EliasGamma(n + 1);

    /// <summary>An unbounded integer of either sign: a sign bit, then its magnitude.</summary>
    public static double SignedInteger(long n) => 1 + Count(Math.Abs(n));

    /// <summary>
    /// The length of a sequence of choices from an alphabet of <paramref name="alphabetSize"/> symbols, coded
    /// adaptively: each choice costs -log2 of its Laplace estimate (count so far + 1) / (choices so far +
    /// alphabet size). Depends only on how often each symbol was chosen, not the order, so it has a closed form -
    /// log2 of (N + K - 1)! / ((K - 1)! · Π nᵢ!). Unlike a code built from the final frequencies, it needs no
    /// frequency table sent ahead of it: the cost of learning the distribution is already inside it.
    /// </summary>
    public static double Adaptive(IEnumerable<int> counts, long alphabetSize)
    {
        long total = 0;
        double symbolTerms = 0;

        foreach (var count in counts)
        {
            total += count;
            symbolTerms += Log2Factorial(count);
        }

        return total == 0 ? 0 : Log2Factorial(total + alphabetSize - 1) - Log2Factorial(alphabetSize - 1) - symbolTerms;
    }

    /// <summary>One choice's share of an <see cref="Adaptive"/> code, estimated from final counts: -log2((count + 1) / (total + alphabet size)).</summary>
    public static double AdaptiveShare(int count, long total, long alphabetSize) =>
        -Math.Log2((count + 1.0) / (total + alphabetSize));

    /// <summary>log2(<paramref name="n"/>!).</summary>
    public static double Log2Factorial(long n)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n), n, "Factorials are of non-negative integers");

        lock (_gate)
        {
            for (var i = _log2Factorials.Count; i <= n; i++)
                _log2Factorials.Add(_log2Factorials[i - 1] + Math.Log2(i));

            return _log2Factorials[(int)n];
        }
    }
}
