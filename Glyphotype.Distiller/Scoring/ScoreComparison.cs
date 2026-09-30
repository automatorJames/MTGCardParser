namespace Glyphotype.Distiller.Scoring;

/// <summary>One glyph's contribution under two grammars - either side null where the glyph doesn't exist.</summary>
public sealed record GlyphContributionChange(string Name, GlyphContribution Before, GlyphContribution After)
{
    public double NetBitsDelta => (After?.NetBits ?? 0) - (Before?.NetBits ?? 0);
}

/// <summary>Two scores of the same corpus, side by side: what changed, in bits and in coverage, and for which glyphs.</summary>
public sealed record ScoreComparison(MdlScore Before, MdlScore After)
{
    /// <summary>Negative is better: the change made the whole description shorter.</summary>
    public double TotalBitsDelta => After.TotalBits - Before.TotalBits;

    public double GrammarBitsDelta => After.GrammarBits - Before.GrammarBits;

    public IReadOnlyDictionary<DataComponent, double> ComponentBitsDelta =>
        Before.ComponentBits.Keys.ToDictionary(x => x, x => After.ComponentBits[x] - Before.ComponentBits[x]);

    public int CapturedWordsDelta => After.CapturedWords - Before.CapturedWords;

    /// <summary>The glyphs whose contribution changed by at least half a bit, or that were added or removed - largest change first.</summary>
    public IReadOnlyList<GlyphContributionChange> GlyphChanges
    {
        get
        {
            var before = Before.Glyphs.ToDictionary(x => x.Name);
            var after = After.Glyphs.ToDictionary(x => x.Name);

            return before.Keys.Union(after.Keys)
                .Select(x => new GlyphContributionChange(x, before.GetValueOrDefault(x), after.GetValueOrDefault(x)))
                .Where(x => x.Before is null || x.After is null || Math.Abs(x.NetBitsDelta) >= 0.5 || x.Before.Occurrences != x.After.Occurrences)
                .OrderByDescending(x => Math.Abs(x.NetBitsDelta))
                .ToList();
        }
    }

    public string ToReport(int rows = 12)
    {
        var report = new StringBuilder();
        var verdict = TotalBitsDelta < -0.5 ? "better" : TotalBitsDelta > 0.5 ? "worse" : "no change";

        report.AppendLine($"Total: {Before.TotalBits:N0} → {After.TotalBits:N0} bits ({Signed(TotalBitsDelta)}, {Signed(100 * TotalBitsDelta / Before.BaselineBits, "N2")}% of the no-grammar baseline) - {verdict}");
        report.AppendLine($"  grammar {Signed(GrammarBitsDelta)} · {string.Join(" · ", ComponentBitsDelta.Select(x => $"{x.Key.ToString().ToLowerInvariant()} {Signed(x.Value)}"))}");
        report.AppendLine($"Coverage: {Before.Coverage:P2} → {After.Coverage:P2} ({Signed(CapturedWordsDelta)} words); now {After.CompressionRatio:P2} of baseline");

        var changes = GlyphChanges;

        if (changes.Count == 0)
            return report.ToString();

        report.AppendLine($"Glyph contributions that changed ({changes.Count}):");
        report.AppendLine($"  {"Δnet",9} {"net",9} {"matches",13}  glyph");

        foreach (var change in changes.Take(rows))
        {
            var matches = $"{change.Before?.Occurrences.ToString("N0") ?? "-"} → {change.After?.Occurrences.ToString("N0") ?? "-"}";
            var status = change.Before is null ? " (added)" : change.After is null ? " (removed)" : change.After.IsTopLevel ? "" : " (nested only)";

            report.AppendLine($"  {Signed(change.NetBitsDelta),9} {change.After?.NetBits.ToString("N0") ?? "-",9} {matches,13}  {change.Name}{status}");
        }

        if (changes.Count > rows)
            report.AppendLine($"  … and {changes.Count - rows} more");

        return report.ToString();
    }

    static string Signed(double value, string format = "N0") =>
        (value > 0 ? "+" : value < 0 ? "−" : "±") + Math.Abs(value).ToString(format, CultureInfo.InvariantCulture);
}
