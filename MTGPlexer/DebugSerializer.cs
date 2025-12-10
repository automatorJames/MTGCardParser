namespace MTGPlexer;

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public static class DebugSerializer
{
    public static string Serialize(object obj)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        // 1. Handle Enum properties as strings
        settings.Converters.Add(new StringEnumConverter());

        // 2. Custom handling for PropertyInfo
        settings.Converters.Add(new PropertyInfoConverter());

        // 3. Custom handling for System.Type (Short name only)
        settings.Converters.Add(new SystemTypeConverter());

        return JsonConvert.SerializeObject(obj, settings);
    }

    /// <summary>
    /// Custom converter to serialize PropertyInfo with specific fields only.
    /// </summary>
    private class PropertyInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PropertyInfo).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var propertyInfo = (PropertyInfo)value;

            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteValue(propertyInfo.Name);

            writer.WritePropertyName("PropertyType");
            writer.WriteValue(propertyInfo.PropertyType.Name); // Using .Name for consistency with SystemTypeConverter

            writer.WritePropertyName("DeclaringMemberName");
            writer.WriteValue(propertyInfo.DeclaringType?.Name);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;
    }

    /// <summary>
    /// Custom converter to serialize System.Type using only the short name.
    /// </summary>
    private class SystemTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Type).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = (Type)value;
            // Write only the short name (e.g., "String" instead of "System.String")
            writer.WriteValue(type.Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;
    }
}