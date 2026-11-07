using System.Collections.Concurrent;

namespace MTGRulesIR.Lowering;

/// <summary>
/// Hand-written IR for lines the grammar matches only with a card-specific literal (see <c>ICardSpecific</c>): the
/// lowering replaces such a line's abilities with the override's, keyed by card name and line text, so the
/// closure check and the engine see the same records a rule would have produced. Overrides live in
/// <c>Overrides/*.jsonl</c> beside the binaries, one <see cref="LineOverride"/> per line.
/// </summary>
public sealed class LineOverrides
{
    readonly Dictionary<(string Card, string Text), LineOverride> _byLine = [];

    public static readonly LineOverrides Empty = new();

    static readonly Lazy<LineOverrides> _default = new(() => Load(Path.Combine(AppContext.BaseDirectory, "Overrides")));

    /// <summary>The overrides shipped with the lowering: every <c>*.jsonl</c> under <c>Overrides</c> beside the binaries.</summary>
    public static LineOverrides Default => _default.Value;

    public int Count => _byLine.Count;

    public IEnumerable<LineOverride> All => _byLine.Values;

    public static LineOverrides Load(string directory)
    {
        var overrides = new LineOverrides();
        if (!Directory.Exists(directory))
            return overrides;
        foreach (var path in Directory.EnumerateFiles(directory, "*.jsonl").OrderBy(x => x, StringComparer.Ordinal))
            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                    continue;
                overrides.Add(IrJson.Deserialize<LineOverride>(line));
            }
        return overrides;
    }

    public void Add(LineOverride item) =>
        _byLine[KeyOf(item.Card, item.Text)] = item;

    /// <summary>The override for <paramref name="card"/>'s line reading <paramref name="text"/>, if one is written.</summary>
    public bool TryGet(string card, string text, out LineOverride found) =>
        _byLine.TryGetValue(KeyOf(card, text), out found);

    static (string, string) KeyOf(string card, string text) =>
        ((card ?? "").Trim().ToLowerInvariant(), Normalize(text));

    static string Normalize(string text) =>
        string.Join(' ', (text ?? "").Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}

/// <summary>One overridden line: the card, the line's text as the corpus has it, the abilities it lowers to, and a note on why the grammar's literal was replaced by hand.</summary>
public sealed record LineOverride(string Card, string Text, IReadOnlyList<Ability> Abilities, string Note = null);
