using System.Collections.Generic;
using System.Net.Http.Json;

namespace TheCodePatch.PinMeTo.Client.Serialization;

internal interface ISerializer
{
    T Deserialize<T>(string serialized);
    string Serialize<T>(T data);
    JsonContent MakeJson<T>(T data);
}
