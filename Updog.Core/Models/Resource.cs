using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updog.Core.Models;

public abstract class Resource
{
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
