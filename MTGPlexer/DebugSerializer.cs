namespace MTGPlexer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

public static class DebugSerializer
{
    public static string Serialize(object obj, params string[] propsToIgnore)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnoreRegexMatchAndNamedPropertiesContractResolver(propsToIgnore)
        };

        // 1. Handle Enum properties as strings
        settings.Converters.Add(new StringEnumConverter());

        // 2. Custom handling for PropertyInfo
        settings.Converters.Add(new PropertyInfoConverter());

        // 3. Custom handling for System.Type (short name only)
        settings.Converters.Add(new SystemTypeConverter());

        return JsonConvert.SerializeObject(obj, settings);
    }

    public static string Serialize(object obj) => Serialize(obj, Array.Empty<string>());

    /// <summary>
    /// Excludes Regex Match members and caller-specified property names
    /// (globally across the object graph).
    /// </summary>
    private sealed class IgnoreRegexMatchAndNamedPropertiesContractResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _ignoredPropertyNames;

        public IgnoreRegexMatchAndNamedPropertiesContractResolver(IEnumerable<string> propsToIgnore)
        {
            _ignoredPropertyNames = new HashSet<string>(
                propsToIgnore?.Where(p => !string.IsNullOrWhiteSpace(p))
                             ?? Enumerable.Empty<string>(),
                StringComparer.Ordinal);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            // 1) Ignore Regex.Match (and derived types)
            if (typeof(Match).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = _ => false;
                return property;
            }

            // 2) Ignore properties by name (anywhere in the graph)
            if (_ignoredPropertyNames.Contains(property.PropertyName))
            {
                property.ShouldSerialize = _ => false;
            }

            return property;
        }
    }

    /// <summary>
    /// Custom converter to serialize PropertyInfo with specific fields only.
    /// </summary>
    private class PropertyInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(PropertyInfo).IsAssignableFrom(objectType);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var propertyInfo = (PropertyInfo)value;

            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteValue(propertyInfo.Name);

            writer.WritePropertyName("PropertyType");
            writer.WriteValue(propertyInfo.PropertyType.Name);

            writer.WritePropertyName("DeclaringMemberName");
            writer.WriteValue(propertyInfo.DeclaringType?.Name);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override bool CanRead => false;
    }

    /// <summary>
    /// Custom converter to serialize System.Type using only the short name.
    /// </summary>
    private class SystemTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(Type).IsAssignableFrom(objectType);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = (Type)value;
            writer.WriteValue(type.Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override bool CanRead => false;
    }
}
