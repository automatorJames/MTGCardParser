using System.Text;

namespace MTGRulesIR.Lowering;

/// <summary>
/// What lowering a corpus left opaque, by glyph type: how many lines each blocks, so the next rule to write is the
/// one at the top. The lowering's counterpart of the grammar's unmatched-text report.
/// </summary>
public sealed class LoweringReport
{
    sealed class Blocker
    {
        public int Occurrences;
        public HashSet<string> Lines = [];
        public List<string> Examples = [];
    }

    readonly Dictionary<string, Blocker> _blockers = [];

    public int Cards { get; private set; }
    public int CardsFullyLowered { get; private set; }
    public int Lines { get; private set; }
    public int LinesFullyLowered { get; private set; }
    public int Abilities { get; private set; }
    public int OpaqueAbilities { get; private set; }

    public void Add(CardRules card)
    {
        Cards++;
        if (card.FullyLowered)
            CardsFullyLowered++;

        foreach (var line in card.Lines)
        {
            Lines++;
            Abilities += line.Abilities.Count;
            OpaqueAbilities += line.Abilities.Count(x => x is OpaqueAbility);

            if (line.FullyLowered)
            {
                LinesFullyLowered++;
                continue;
            }

            var lineKey = $"{card.Name}/{line.Line}";

            foreach (var opaque in line.Abilities.SelectMany(OpaqueNodes.In))
            {
                if (!_blockers.TryGetValue(opaque.Glyph, out var blocker))
                    _blockers[opaque.Glyph] = blocker = new();

                blocker.Occurrences++;
                blocker.Lines.Add(lineKey);

                if (blocker.Examples.Count < 3 && !blocker.Examples.Contains(opaque.Text))
                    blocker.Examples.Add(opaque.Text);
            }
        }
    }

    public string ToText(int top = 60)
    {
        var text = new StringBuilder();
        text.AppendLine($"Cards: {Cards:N0}, fully lowered {CardsFullyLowered:N0} ({Share(CardsFullyLowered, Cards)})");
        text.AppendLine($"Lines: {Lines:N0}, fully lowered {LinesFullyLowered:N0} ({Share(LinesFullyLowered, Lines)})");
        text.AppendLine($"Abilities: {Abilities:N0}, opaque at the top level {OpaqueAbilities:N0} ({Share(OpaqueAbilities, Abilities)})");
        text.AppendLine();
        text.AppendLine($"Opaque glyph types, by lines blocked (top {top} of {_blockers.Count}):");

        foreach (var (glyph, blocker) in _blockers.OrderByDescending(x => x.Value.Lines.Count).ThenByDescending(x => x.Value.Occurrences).Take(top))
            text.AppendLine($"{blocker.Lines.Count,7} lines {blocker.Occurrences,7}x  {glyph}  e.g. {string.Join(" | ", blocker.Examples.Select(x => x.Length > 70 ? x[..70] + "…" : x))}");

        return text.ToString();
    }

    static string Share(int part, int whole) => whole == 0 ? "-" : $"{(double)part / whole:P1}";
}
