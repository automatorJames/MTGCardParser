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
/// free text, by <c>Record.Property</c>, and the distinct values it took with how many lines each appeared in. Each
/// slot is a vocabulary the engine will switch on; a slot's list is its to-do list for becoming an enum, and a long
/// tail in it is a slot being used as an escape hatch. Free text - card text, names, glyph type names, bindings - is
/// left out by property name.
/// </summary>
public sealed class VocabularyCensus
{
    static readonly HashSet<string> _freeText = ["Text", "Name", "Glyph", "Binding", "Antecedent", "Types"];

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
        var slot = node.GetType().Name + "." + property.Name;

        if (!_slots.TryGetValue(slot, out var values))
            _slots[slot] = values = [];

        if (!values.TryGetValue(value, out var lines))
            values[value] = lines = [];

        lines.Add(lineKey);
    }

    /// <summary>The slots, their distinct values, and how many lines each value appeared in.</summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> Slots =>
        _slots.ToDictionary(x => x.Key, x => (IReadOnlyDictionary<string, int>)x.Value.ToDictionary(v => v.Key, v => v.Value.Count));

    /// <summary>One section per slot, widest vocabulary first; within a slot, values by lines, descending.</summary>
    public string ToText(int valuesPerSlot = 200)
    {
        var text = new StringBuilder();
        text.AppendLine($"String slots: {_slots.Count}, distinct values {_slots.Sum(x => x.Value.Count):N0}");

        foreach (var (slot, values) in _slots.OrderByDescending(x => x.Value.Count).ThenBy(x => x.Key, StringComparer.Ordinal))
        {
            text.AppendLine();
            text.AppendLine($"{slot}: {values.Count} values, {values.Values.SelectMany(x => x).Distinct().Count():N0} lines");

            foreach (var (value, lines) in values.OrderByDescending(x => x.Value.Count).ThenBy(x => x.Key, StringComparer.Ordinal).Take(valuesPerSlot))
                text.AppendLine($"{lines.Count,8}  {value}");

            if (values.Count > valuesPerSlot)
                text.AppendLine($"          ... {values.Count - valuesPerSlot} more");
        }

        return text.ToString();
    }
}
