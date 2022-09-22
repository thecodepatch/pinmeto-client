using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Serialization;

internal class JsonDateOnlyConverter
{
    private static DateOnly Parse(string s)
    {
        return DateOnly.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static string Serialize(DateOnly d)
    {
        return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public class NonNullable : JsonConverter<DateOnly>
    {
        public override bool HandleNull => true;

        public override DateOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var val = reader.GetString();
            return Parse(val!);
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly dateOnlyValue,
            JsonSerializerOptions options
        )
        {
            writer.WriteStringValue(Serialize(dateOnlyValue));
        }
    }

    public class Nullable : JsonConverter<DateOnly?>
    {
        public override bool HandleNull => true;

        public override DateOnly? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var val = reader.GetString();
            return string.IsNullOrWhiteSpace(val) ? null : Parse(val);
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly? dateOnlyValue,
            JsonSerializerOptions options
        )
        {
            var val = dateOnlyValue.HasValue ? Serialize(dateOnlyValue.Value) : null;
            writer.WriteStringValue(val);
        }
    }
}
