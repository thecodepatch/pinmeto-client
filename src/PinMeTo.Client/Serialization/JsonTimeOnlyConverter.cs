using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheCodePatch.PinMeTo.Client.Serialization;

internal class JsonTimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => TimeOnly.ParseExact(reader.GetString()!, "HHmm", CultureInfo.InvariantCulture);

    public override void Write(
        Utf8JsonWriter writer,
        TimeOnly timeOnlyValue,
        JsonSerializerOptions options
    ) => writer.WriteStringValue(timeOnlyValue.ToString("HHmm", CultureInfo.InvariantCulture));
}