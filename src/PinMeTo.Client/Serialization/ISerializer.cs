using System.Collections.Generic;
using System.Net.Http.Json;

namespace TheCodePatch.PinMeTo.Client.Serialization;

internal interface ISerializer
{
    T Deserialize<T>(string serialized);
    JsonContent MakeJson<T>(T data);
}
