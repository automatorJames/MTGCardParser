using System.Text.Json;
using System.Text.Json.Nodes;
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

    public static T Deserialize<T>(string json) =>
        JsonNode.Parse(json) is { } node ? Migrate(node).Deserialize<T>(_options) : default;

    /// <summary>
    /// Brings JSON saved by an earlier version of the definitions up to date, wherever it sits in <paramref name="node"/>
    /// (a grammar, a history of them, ...), so a saved workspace still loads:
    /// <list type="bullet">
    /// <item><c>Introduces</c> became <see cref="GlyphDefinition.IsReferent"/>/<see cref="PropertyDefinition.IsReferent"/>
    /// and <c>Number</c>; <c>Agreement</c> became <c>Number</c>. Their kinds were text, and kinds are now types (see
    /// <see cref="BackReference{T}"/>), so those are dropped - a back-reference that named one refers to any kind until
    /// it's given a <c>T</c>.</item>
    /// <item>A plural nib was a suffix on the nib before it; now it wraps that nib (see <see cref="NibDefinition.Plural"/>).</item>
    /// </list>
    /// </summary>
    static JsonNode Migrate(JsonNode node)
    {
        switch (node)
        {
            case JsonObject obj:
                if (obj["Introduces"] is JsonObject introduces)
                {
                    obj["IsReferent"] = true;
                    MoveNumber(introduces, obj);
                }

                if (obj["Agreement"] is JsonObject agreement)
                    MoveNumber(agreement, obj);

                obj.Remove("Introduces");
                obj.Remove("Agreement");

                foreach (var (_, child) in obj.ToList())
                    if (child is not null)
                        Migrate(child);

                break;

            case JsonArray array:
                for (var i = array.Count - 1; i > 0; i--)
                    if (array[i] is JsonObject { Count: 1 } suffix && suffix["$nib"]?.GetValue<string>() == "plural" && array[i - 1] is JsonObject before)
                    {
                        array.RemoveAt(i);
                        array.RemoveAt(i - 1);
                        array.Insert(i - 1, new JsonObject { ["$nib"] = "plural", ["Inner"] = before });
                    }

                foreach (var item in array)
                    if (item is not null)
                        Migrate(item);

                break;
        }

        return node;

        static void MoveNumber(JsonObject from, JsonObject to)
        {
            if (from["Number"]?.GetValue<string>() is string number && number != nameof(GrammaticalNumber.Unspecified))
                to["Number"] = number;
        }
    }

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
