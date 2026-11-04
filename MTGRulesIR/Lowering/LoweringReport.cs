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
    readonly Dictionary<string, Blocker> _openSlots = [];

    public int Cards { get; private set; }
    public int CardsFullyLowered { get; private set; }
    public int Lines { get; private set; }
    public int LinesFullyLowered { get; private set; }
    public int CardsEngineClosed { get; private set; }
    public int LinesEngineClosed { get; private set; }
    public int Abilities { get; private set; }
    public int OpaqueAbilities { get; private set; }

    public void Add(CardRules card)
    {
        Cards++;
        if (card.FullyLowered)
            CardsFullyLowered++;
        if (card.EngineClosed)
            CardsEngineClosed++;

        foreach (var line in card.Lines)
        {
            Lines++;
            Abilities += line.Abilities.Count;
            OpaqueAbilities += line.Abilities.Count(x => x is OpaqueAbility);

            if (line.FullyLowered)
                LinesFullyLowered++;
            if (line.EngineClosed)
            {
                LinesEngineClosed++;
                continue;
            }

            var lineKey = $"{card.Name}/{line.Line}";

            foreach (var opaque in line.Abilities.SelectMany(OpaqueNodes.In))
                Count(_blockers, opaque.Glyph, lineKey, opaque.Text);

            // Open slots count toward engine closure only once the line is fully lowered: an untyped slot holding an
            // opaque node is the opaque node's problem, and typing the slot wouldn't close the line.
            if (line.FullyLowered)
                foreach (var slot in line.Open ?? [])
                    Count(_openSlots, slot, lineKey, line.Text);
        }
    }

    static void Count(Dictionary<string, Blocker> blockers, string key, string lineKey, string example)
    {
        if (!blockers.TryGetValue(key, out var blocker))
            blockers[key] = blocker = new();

        blocker.Occurrences++;
        blocker.Lines.Add(lineKey);

        if (blocker.Examples.Count < 3 && !blocker.Examples.Contains(example))
            blocker.Examples.Add(example);
    }

    public string ToText(int top = 60)
    {
        var text = new StringBuilder();
        text.AppendLine($"Cards: {Cards:N0}, fully lowered {CardsFullyLowered:N0} ({Share(CardsFullyLowered, Cards)}), engine-closed {CardsEngineClosed:N0} ({Share(CardsEngineClosed, Cards)})");
        text.AppendLine($"Lines: {Lines:N0}, fully lowered {LinesFullyLowered:N0} ({Share(LinesFullyLowered, Lines)}), engine-closed {LinesEngineClosed:N0} ({Share(LinesEngineClosed, Lines)})");
        text.AppendLine($"Abilities: {Abilities:N0}, opaque at the top level {OpaqueAbilities:N0} ({Share(OpaqueAbilities, Abilities)})");
        text.AppendLine();
        text.AppendLine($"Opaque glyph types, by lines blocked (top {top} of {_blockers.Count}):");
        Append(text, _blockers, top);
        text.AppendLine();
        text.AppendLine($"Open slots in fully lowered lines, by lines kept from engine closure (top {top} of {_openSlots.Count}):");
        Append(text, _openSlots, top);

        return text.ToString();
    }

    static void Append(StringBuilder text, Dictionary<string, Blocker> blockers, int top)
    {
        foreach (var (key, blocker) in blockers.OrderByDescending(x => x.Value.Lines.Count).ThenByDescending(x => x.Value.Occurrences).Take(top))
            text.AppendLine($"{blocker.Lines.Count,7} lines {blocker.Occurrences,7}x  {key}  e.g. {string.Join(" | ", blocker.Examples.Select(x => x.Length > 70 ? x[..70] + "…" : x))}");
    }

    static string Share(int part, int whole) => whole == 0 ? "-" : $"{(double)part / whole:P1}";
}
