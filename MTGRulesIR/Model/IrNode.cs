using System.Text.Json.Serialization.Metadata;

namespace MTGRulesIR.Model;

/// <summary>
/// A node of the rules intermediate representation: what a card's text means, in the engine's terms, with the
/// grammar's syntax lowered away. Every node is a record, so two lowerings of the same text compare equal, and the
/// whole tree serializes to JSON (see <see cref="IrJson"/>) with a <c>$kind</c> discriminator per node.
/// <para>
/// The families - <see cref="Ability"/>, <see cref="Effect"/>, <see cref="Selector"/>, <see cref="Quantity"/>,
/// <see cref="Duration"/>, <see cref="Condition"/>, <see cref="Event"/>, <see cref="Cost"/> - are the slots the
/// engine's interfaces take. Each has an opaque member (see <see cref="IOpaque"/>) that holds text the lowering
/// couldn't yet express, so a partly lowered tree is still well-formed and says exactly where it stopped.
/// </para>
/// </summary>
public abstract record IrNode;

/// <summary>A node standing for text the lowering has no rule for yet: the text, and the glyph type it came from.</summary>
public interface IOpaque
{
    string Text { get; }
    string Glyph { get; }
}

/// <summary>An opaque node in a slot of no particular family.</summary>
public sealed record OpaqueNode(string Text, string Glyph) : IrNode, IOpaque;

/// <summary>A vocabulary member as itself: the member's name and its vocabulary. The terminal most slots end in.</summary>
public sealed record Term(string Value, string Vocabulary) : IrNode;

/// <summary>JSON for the IR: enums by name, defaults left out, and every abstract family written with a <c>$kind</c> discriminator naming the concrete record.</summary>
public static class IrJson
{
    static readonly Type[] _nodeTypes = typeof(IrNode).Assembly.GetTypes()
        .Where(x => x.IsAssignableTo(typeof(IrNode)) && !x.IsAbstract && x.IsPublic)
        .OrderBy(x => x.Name, StringComparer.Ordinal)
        .ToArray();

    public static readonly JsonSerializerOptions Options = new()
    {
        // Nulls are left out; zeros and falses are written, since an enum's first member (Quantifier.Target, PlayerKind.You) is often the common case, not an absence.
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { AddPolymorphism } },
    };

    public static readonly JsonSerializerOptions Indented = new(Options) { WriteIndented = true };

    public static string Serialize<T>(T value, bool indented = false) => JsonSerializer.Serialize(value, indented ? Indented : Options);

    public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);

    static void AddPolymorphism(JsonTypeInfo info)
    {
        if (!info.Type.IsAssignableTo(typeof(IrNode)) || !info.Type.IsAbstract)
            return;

        info.PolymorphismOptions = new() { TypeDiscriminatorPropertyName = "$kind" };

        foreach (var type in _nodeTypes.Where(x => x.IsAssignableTo(info.Type)))
            info.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(type, type.Name));
    }
}
