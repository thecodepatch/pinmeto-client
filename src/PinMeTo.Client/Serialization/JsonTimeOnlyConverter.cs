using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Serialization;

internal class JsonTimeOnlyConverter
{
    private static TimeOnly Parse(string s)
    {
        if (s == "2400")
        {
            return TimeOnly.MaxValue;
        }
        return TimeOnly.ParseExact(s, "HHmm", CultureInfo.InvariantCulture);
    }

    private static string Serialize(TimeOnly val)
    {
        return val.ToString("HHmm", CultureInfo.InvariantCulture);
    }

    public class Nullable : JsonConverter<TimeOnly?>
    {
        public override bool HandleNull => true;

        public override TimeOnly? Read(
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
            TimeOnly? timeOnlyValue,
            JsonSerializerOptions options
        )
        {
            var val = timeOnlyValue.HasValue ? Serialize(timeOnlyValue.Value) : null;
            writer.WriteStringValue(val);
        }
    }

    public class NonNullable : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(
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
            TimeOnly timeOnlyValue,
            JsonSerializerOptions options
        )
        {
            var val = Serialize(timeOnlyValue);
            writer.WriteStringValue(val);
        }
    }
}
