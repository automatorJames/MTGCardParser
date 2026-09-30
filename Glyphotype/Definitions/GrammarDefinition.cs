using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Glyphotype.Definitions;

/// <summary>
/// A set of <see cref="GlyphDefinition"/>s and the vocabularies and markers they refer to - the portable,
/// serializable source of a grammar, which <see cref="GlyphGrammar.FromDefinition"/> compiles into a runnable
/// <see cref="GlyphGrammar"/>. A reference to a name defined nowhere here must be resolvable when it's compiled
/// (see <see cref="GrammarEmitter.Emit"/>).
/// </summary>
public sealed record GrammarDefinition
{
    public IReadOnlyList<GlyphDefinition> Glyphs { get; init; } = [];

    public IReadOnlyList<VocabularyDefinition> Vocabularies { get; init; } = [];

    /// <summary>
    /// Marker interfaces: member-less tags a glyph can carry (<see cref="GlyphDefinition.Markers"/>) so a
    /// <see cref="DynamicGlyph"/> property's <see cref="PropertyDefinition.TypeFilter"/> can select it.
    /// </summary>
    public IReadOnlyList<string> Markers { get; init; } = [];

    /// <summary>Reads the definitions of <paramref name="glyphTypes"/>, plus the vocabularies and markers they refer to. Generic primitives among them are skipped: they're referred to, not defined.</summary>
    public static GrammarDefinition FromTypes(IEnumerable<Type> glyphTypes) =>
        DefinitionReader.ReadGrammar(glyphTypes);

    static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { OmitEmptyLists } },
    };

    public string ToJson() => JsonSerializer.Serialize(this, _jsonOptions);

    public static GrammarDefinition FromJson(string json) => JsonSerializer.Deserialize<GrammarDefinition>(json, _jsonOptions);

    /// <summary>
    /// Leaves empty lists out of the JSON, as <see cref="JsonIgnoreCondition.WhenWritingDefault"/> does for other
    /// defaults: a list property left out reads back as its <c>[]</c> initializer. Positional records (the
    /// <see cref="NibDefinition"/> cases) are exempt, since a constructor parameter left out would read back null.
    /// </summary>
    static void OmitEmptyLists(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object || typeInfo.Type.IsAssignableTo(typeof(NibDefinition)))
            return;

        foreach (var property in typeInfo.Properties)
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
                property.ShouldSerialize = (_, value) => value is System.Collections.IEnumerable list && list.Cast<object>().Any();
    }
}
