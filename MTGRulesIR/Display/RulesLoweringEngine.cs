using System.Collections;
using Glyphotype.Lowering;
using MTGRulesIR.Lowering;

namespace MTGRulesIR.Display;

/// <summary>
/// The rules IR as an <see cref="ILoweringEngine"/>: lowers a card and describes the result as the generic tree the
/// analysis app shows - each record a node of its family, its properties the fields, opaque nodes, terms and
/// references marked as such, and each node placed in the line's text where its glyph was.
/// </summary>
public sealed class RulesLoweringEngine : ILoweringEngine
{
    readonly CorpusLowering _lowering;

    public RulesLoweringEngine(CorpusLowering lowering = null)
    {
        _lowering = lowering ?? new CorpusLowering();
    }

    public string Name => "Rules IR";

    /// <summary>The IR's families, as <see cref="IrNode"/>'s abstract subclasses name them, with "Part" for the records that are pieces of others (a filter, a damage portion, a counter spec).</summary>
    public IReadOnlyList<string> Families { get; } =
        [nameof(Ability), nameof(Effect), nameof(Selector), nameof(Quantity), nameof(Duration), nameof(Condition), nameof(Event), nameof(Cost), PartFamily];

    const string PartFamily = "Part";

    public LoweredDocument Lower(ProcessedDocument document)
    {
        var (rules, origins) = _lowering.LowerWithOrigins(document);
        List<LoweredLine> lines = [];

        for (int i = 0; i < rules.Lines.Count; i++)
        {
            var line = rules.Lines[i];
            var builder = new NodeBuilder(origins[i].Spans);
            lines.Add(new LoweredLine(line.Line, line.Text, line.Abilities.Select(x => builder.Build(x)).ToList(), line.Opaque, origins[i].UnboundReferences));
        }

        return new LoweredDocument(document.Document, lines, IrJson.Serialize(rules, indented: true));
    }

    public LoweredSummary Summarize(ProcessedDocument document)
    {
        var (rules, origins) = _lowering.LowerWithOrigins(document);
        List<LoweredLineSummary> lines = [];

        for (int i = 0; i < rules.Lines.Count; i++)
        {
            var line = rules.Lines[i];
            var opaque = line.Abilities.SelectMany(OpaqueNodes.In).ToList();
            lines.Add(new LoweredLineSummary(line.Line, opaque.Select(x => x.Glyph).ToList(), opaque.Select(x => x.Text).ToList(), origins[i].UnboundReferences));
        }

        return new LoweredSummary(document.Document, lines);
    }

    /// <summary>Turns one line's IR records into <see cref="LoweredNode"/>s, reading each record's properties by reflection.</summary>
    sealed class NodeBuilder(IReadOnlyDictionary<IrNode, SourceSpan> spans)
    {
        static readonly Dictionary<Type, PropertyInfo[]> _properties = [];
        static readonly HashSet<string> _hiddenProperties = [nameof(ObjectSelector.Binding)];

        public LoweredNode Build(IrNode node)
        {
            var type = node.GetType();
            var span = spans.TryGetValue(node, out var found) ? found : (SourceSpan?)null;

            return node switch
            {
                IOpaque opaque => new LoweredNode(type.Name, FamilyOf(type), LoweredNodeStatus.Opaque, [], opaque.Text, opaque.Glyph, span),
                Term term => new LoweredNode(type.Name, FamilyOf(type), LoweredNodeStatus.Term, [], term.Value, term.Vocabulary, span),
                Reference reference => new LoweredNode(type.Name, FamilyOf(type), LoweredNodeStatus.Reference, [], reference.Text, reference.Antecedent, span, RefersTo: reference.Binding),
                _ => new LoweredNode(type.Name, FamilyOf(type), LoweredNodeStatus.Lowered, FieldsOf(node, type), Span: span, Binding: BindingOf(node)),
            };
        }

        List<LoweredField> FieldsOf(IrNode node, Type type)
        {
            List<LoweredField> fields = [];

            foreach (var property in PropertiesOf(type))
            {
                var value = property.GetValue(node);

                switch (value)
                {
                    case null:
                        continue;
                    case IrNode child:
                        fields.Add(new LoweredField(property.Name, Nodes: [Build(child)]));
                        continue;
                    case string text:
                        fields.Add(new LoweredField(property.Name, text));
                        continue;
                    case bool flag:
                        if (flag)
                            fields.Add(new LoweredField(property.Name, "true"));
                        continue;
                    case IEnumerable list:
                        var items = list.Cast<object>().Where(x => x is not null).ToList();
                        if (items.Count == 0)
                            continue;
                        if (items.All(x => x is IrNode))
                            fields.Add(new LoweredField(property.Name, Nodes: items.Cast<IrNode>().Select(Build).ToList()));
                        else
                            fields.Add(new LoweredField(property.Name, string.Join(", ", items)));
                        continue;
                    default:
                        fields.Add(new LoweredField(property.Name, value.ToString()));
                        continue;
                }
            }

            return fields;
        }

        static PropertyInfo[] PropertiesOf(Type type)
        {
            lock (_properties)
            {
                if (!_properties.TryGetValue(type, out var properties))
                    _properties[type] = properties = type.GetProperties()
                        .Where(x => x.GetIndexParameters().Length == 0 && !_hiddenProperties.Contains(x.Name) && x.Name is not nameof(LineRules.FullyLowered))
                        .ToArray();

                return properties;
            }
        }

        static string BindingOf(IrNode node) =>
            node switch
            {
                ObjectSelector selector => selector.Binding,
                PlayerSelector player => player.Binding,
                _ => null,
            };

        /// <summary>The abstract family <paramref name="type"/> derives from, or "Part" for a record that's a piece of others.</summary>
        static string FamilyOf(Type type)
        {
            for (var current = type.BaseType; current is not null && current != typeof(IrNode); current = current.BaseType)
                if (current.IsAbstract)
                    return current.Name;

            return PartFamily;
        }
    }
}
