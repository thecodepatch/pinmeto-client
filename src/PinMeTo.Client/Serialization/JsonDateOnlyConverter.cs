using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Serialization;

internal class JsonDateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return DateOnly.ParseExact(reader.GetString()!, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly dateOnlyValue,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(dateOnlyValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}