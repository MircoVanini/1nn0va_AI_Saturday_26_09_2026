using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace DemoClassLibrary
{
    public static class DemoClass
    {
        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            },
        };

        public static string Serialize(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.GetType() == typeof(object))
                throw new ArgumentException("Specified value is not an object instance", nameof(value));

            return JsonSerializer.Serialize(value, SerializerOptions);
        }

        public static T? Deserialize<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            return JsonSerializer.Deserialize<T>(value, SerializerOptions);
        }

        public static object? Deserialize(string value, Type dataType)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            if (dataType == null)
                throw new ArgumentNullException(nameof(dataType));

            return JsonSerializer.Deserialize(value, dataType, SerializerOptions);
        }

        public static T? DeserializeFromStream<T>(StreamReader streamReader)
        {
            if (streamReader == null)
                throw new ArgumentNullException(nameof(streamReader));

            return JsonSerializer.Deserialize<T>(streamReader.ReadToEnd(), SerializerOptions);
        }
    }
}
