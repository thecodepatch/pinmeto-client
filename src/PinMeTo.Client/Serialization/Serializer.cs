using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TheCodePatch.PinMeTo.Client.Exceptions;

namespace TheCodePatch.PinMeTo.Client.Serialization;

internal class Serializer : ISerializer
{
    private readonly JsonSerializerOptions _options =
        new()
        {
            Converters =
            {
                new JsonStringEnumConverter(),
                new JsonDateOnlyConverter.Nullable(),
                new JsonDateOnlyConverter.NonNullable(),
                new JsonTimeOnlyConverter.Nullable(),
                new JsonTimeOnlyConverter.NonNullable(),
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

    public T Deserialize<T>(string serialized)
    {
        return JsonSerializer.Deserialize<T>(serialized, _options)
            ?? throw new DeserializationException();
    }

    public JsonContent MakeJson<T>(T data)
    {
        return JsonContent.Create(data, null, _options);
    }
}
