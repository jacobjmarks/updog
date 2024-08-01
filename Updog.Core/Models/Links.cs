using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updog.Core.Models;

public sealed class Links
{
    public string? Prev { get; set; }
    public string? Next { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
