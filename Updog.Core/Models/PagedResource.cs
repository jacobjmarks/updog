using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updog.Core.Models;

public sealed class PagedResource<T>
{
    public IEnumerable<T> Data { get; set; } = [];

    public Links Links { get; set; } = new();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
