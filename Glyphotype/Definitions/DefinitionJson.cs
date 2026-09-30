using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Glyphotype.Definitions;

/// <summary>
/// The JSON form of definitions - a whole <see cref="GrammarDefinition"/> or any one definition in it: indented,
/// enums by name, and defaults (null, false, empty lists) left out.
/// </summary>
public static class DefinitionJson
{
    static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { OmitEmptyLists } },
    };

    public static string Serialize<T>(T definition) => JsonSerializer.Serialize(definition, _options);

    public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);

    /// <summary>
    /// Leaves empty lists out, as <see cref="JsonIgnoreCondition.WhenWritingDefault"/> does for other defaults: a
    /// list property left out reads back as its <c>[]</c> initializer. Positional records (the
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
