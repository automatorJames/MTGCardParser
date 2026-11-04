using System.Collections;
using System.Collections.Concurrent;
using System.Text;

namespace MTGRulesIR.Lowering;

/// <summary>
/// Walks an IR tree: every node, and every slot (a property of a node) with the node it belongs to and the property
/// that holds it. Shared by the opaque search, the open-slot search and the census.
/// </summary>
public static class NodeWalker
{
    static readonly ConcurrentDictionary<Type, PropertyInfo[]> _properties = new();

    public static PropertyInfo[] PropertiesOf(Type type) =>
        _properties.GetOrAdd(type, x => x.GetProperties().Where(p => p.GetIndexParameters().Length == 0 && p.GetMethod?.IsPublic == true).ToArray());

    public static IEnumerable<IrNode> Nodes(IrNode root)
    {
        if (root is null)
            yield break;

        yield return root;

        foreach (var property in PropertiesOf(root.GetType()))
        {
            var value = property.GetValue(root);

            if (value is IrNode child)
                foreach (var found in Nodes(child))
                    yield return found;
            else if (value is IEnumerable list and not string)
                foreach (var item in list)
                    if (item is IrNode element)
                        foreach (var found in Nodes(element))
                            yield return found;
        }
    }

    /// <summary>Every property of every node under <paramref name="root"/>, with its value - lists yielded whole.</summary>
    public static IEnumerable<(IrNode Node, PropertyInfo Property, object Value)> Slots(IrNode root)
    {
        foreach (var node in Nodes(root))
            foreach (var property in PropertiesOf(node.GetType()))
                yield return (node, property, property.GetValue(node));
    }
}

/// <summary>Finds the opaque nodes in an IR tree.</summary>
public static class OpaqueNodes
{
    public static IEnumerable<IOpaque> In(IrNode node) =>
        NodeWalker.Nodes(node).OfType<IOpaque>();
}

/// <summary>
/// What keeps a lowered line from being engine-closed - executable without the engine reading text or guessing at
/// an untyped slot - besides opaque nodes: a slot declared as a bare <see cref="IrNode"/> (or a list of them) that
/// holds something, which is the lowering saying "here is a node I had nowhere typed to put"; and a placeholder
/// value, a string of <c>?</c>, standing in for something the rule couldn't determine. Each is named
/// <c>Record.Property</c>, so the report ranks which slots to type next.
/// </summary>
public static class OpenSlots
{
    public const string Placeholder = "?";

    public static IEnumerable<string> In(IrNode root)
    {
        foreach (var (node, property, value) in NodeWalker.Slots(root))
        {
            if (value is null)
                continue;

            var name = node.GetType().Name + "." + property.Name;

            if (property.PropertyType == typeof(IrNode))
                yield return name;
            else if (property.PropertyType == typeof(IReadOnlyList<IrNode>) && value is ICollection { Count: > 0 })
                yield return name;
            else if (value is string text && text == Placeholder)
                yield return name + "=?";
            else if (value is IReadOnlyList<string> texts && texts.Contains(Placeholder))
                yield return name + "=?";
        }
    }
}

/// <summary>
/// The leaf vocabulary of a lowered corpus: every string-typed slot (a <c>string</c> or a list of them) that isn't
/// free text, by <c>Record.Property</c> - split by vocabulary where the record names one in a <c>Vocabulary</c>
/// property, as <see cref="Term"/> and <see cref="Qualifier"/> do - and the distinct values it took with how many
/// lines each appeared in. A slot whose vocabulary is an enum (a grammar vocabulary, or one of
/// <see cref="Vocabularies"/>) is closed, and only values outside the enum are listed; the rest are the vocabularies
/// still to be closed. Free text - card text, names, glyph type names, bindings - is left out by property name.
/// </summary>
public sealed class VocabularyCensus
{
    static readonly HashSet<string> _freeText = ["Text", "Name", "Glyph", "Binding", "Antecedent", "Types", "Vocabulary"];

    public static bool IsFreeText(PropertyInfo property) => _freeText.Contains(property.Name);

    readonly Dictionary<string, Dictionary<string, HashSet<string>>> _slots = [];

    public void Add(CardRules card)
    {
        foreach (var line in card.Lines)
        {
            var lineKey = $"{card.Name}/{line.Line}";

            foreach (var ability in line.Abilities)
                foreach (var (node, property, value) in NodeWalker.Slots(ability))
                {
                    if (value is null || IsFreeText(property))
                        continue;

                    if (value is string text)
                        Count(node, property, text, lineKey);
                    else if (value is IReadOnlyList<string> texts)
                        foreach (var item in texts)
                            Count(node, property, item, lineKey);
                }
        }
    }

    void Count(IrNode node, PropertyInfo property, string value, string lineKey)
    {
        var slot = SlotOf(node, property);

        if (!_slots.TryGetValue(slot, out var values))
            _slots[slot] = values = [];

        if (!values.TryGetValue(value, out var lines))
            values[value] = lines = [];

        lines.Add(lineKey);
    }

    /// <summary><c>Record.Property</c>, with the node's vocabulary in brackets when it names one: <c>Qualifier.Quality[CardType]</c>.</summary>
    static string SlotOf(IrNode node, PropertyInfo property)
    {
        var slot = node.GetType().Name + "." + property.Name;
        var vocabulary = node.GetType().GetProperty("Vocabulary")?.GetValue(node) as string;
        return vocabulary is null ? slot : $"{slot}[{vocabulary}]";
    }

    /// <summary>The enums a slot's values must belong to - from the vocabulary in its name, or from <see cref="Vocabularies.Slots"/> - or null when the slot is open.</summary>
    public static Type[] EnumsOf(string slot)
    {
        var bracket = slot.IndexOf('[');
        return bracket >= 0
            ? Vocabularies.Find(slot[(bracket + 1)..^1]) is { } found ? [found] : null
            : Vocabularies.Slots.GetValueOrDefault(slot);
    }

    /// <summary>Whether <paramref name="value"/> belongs to the slot's vocabulary: one of its enums, or its pattern.</summary>
    public static bool Belongs(string slot, string value) =>
        EnumsOf(slot) is { } enums ? enums.Any(x => x.IsEnum && Enum.IsDefined(x, value))
        : Vocabularies.Patterns.TryGetValue(slot, out var pattern) && pattern.IsMatch(value);

    /// <summary>Whether the slot has a vocabulary declared, by enum or by pattern.</summary>
    public static bool IsClosed(string slot) =>
        EnumsOf(slot) is not null || Vocabularies.Patterns.ContainsKey(slot);

    static string VocabularyName(string slot) =>
        EnumsOf(slot) is { } enums ? string.Join("|", enums.Select(x => x.Name)) : "pattern";

    /// <summary>The enums that define most of <paramref name="values"/>, as a hint at which vocabulary an open slot could be declared to hold: "  ~ PermanentEvent (78/81), ...".</summary>
    static string Candidates(IReadOnlyCollection<string> values)
    {
        var candidates = Vocabularies.All
            .Select(x => (Enum: x, Covered: values.Count(v => Enum.IsDefined(x, v))))
            .Where(x => x.Covered * 2 >= values.Count)
            .OrderByDescending(x => x.Covered).ThenBy(x => Enum.GetNames(x.Enum).Length)
            .Take(3)
            .ToList();

        return candidates.Count == 0 ? "" : "  ~ " + string.Join(", ", candidates.Select(x => $"{x.Enum.Name} ({x.Covered}/{values.Count})"));
    }

    /// <summary>The slots, their distinct values, and how many lines each value appeared in.</summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> Slots =>
        _slots.ToDictionary(x => x.Key, x => (IReadOnlyDictionary<string, int>)x.Value.ToDictionary(v => v.Key, v => v.Value.Count));

    /// <summary>The values of each slot that lie outside its enum, for the slots that have one.</summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Outsiders =>
        _slots.Where(x => IsClosed(x.Key))
            .ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Value.Keys.Where(v => !Belongs(x.Key, v)).ToList());

    /// <summary>
    /// The open slots first, widest vocabulary first, with their values by lines descending; then the closed slots,
    /// each on a line, with any values outside their enum called out.
    /// </summary>
    public string ToText(int valuesPerSlot = 200)
    {
        var text = new StringBuilder();
        var open = _slots.Where(x => !IsClosed(x.Key)).OrderByDescending(x => x.Value.Count).ThenBy(x => x.Key, StringComparer.Ordinal).ToList();
        var closed = _slots.Where(x => IsClosed(x.Key)).OrderBy(x => x.Key, StringComparer.Ordinal).ToList();
        var outsiders = Outsiders;
        text.AppendLine($"String slots: {_slots.Count}, open {open.Count} with {open.Sum(x => x.Value.Count):N0} distinct values, closed {closed.Count} with {outsiders.Sum(x => x.Value.Count):N0} values outside their vocabulary");

        foreach (var (slot, values) in open)
        {
            text.AppendLine();
            text.AppendLine($"{slot}: {values.Count} values, {values.Values.SelectMany(x => x).Distinct().Count():N0} lines{Candidates(values.Keys)}");

            foreach (var (value, lines) in values.OrderByDescending(x => x.Value.Count).ThenBy(x => x.Key, StringComparer.Ordinal).Take(valuesPerSlot))
                text.AppendLine($"{lines.Count,8}  {value}");

            if (values.Count > valuesPerSlot)
                text.AppendLine($"          ... {values.Count - valuesPerSlot} more");
        }

        text.AppendLine();
        text.AppendLine("Closed slots:");

        foreach (var (slot, values) in closed)
        {
            var outside = outsiders[slot];
            text.AppendLine(outside.Count == 0
                ? $"  {slot}: {values.Count} values, all in {VocabularyName(slot)}"
                : $"  {slot}: {values.Count} values, {outside.Count} outside {VocabularyName(slot)}: {string.Join(", ", outside.OrderByDescending(v => values[v].Count).Take(12))}{(outside.Count > 12 ? ", ..." : "")}");
        }

        return text.ToString();
    }
}
