﻿using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TheCodePatch.PinMeToClient.Exceptions;

namespace TheCodePatch.PinMeToClient.Serialization;

internal class Serializer : ISerializer
{
    private readonly JsonSerializerOptions _options =
        new()
        {
            Converters =
            {
                new JsonStringEnumConverter(),
                new JsonDateOnlyConverter(),
                new JsonTimeOnlyConverter(),
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