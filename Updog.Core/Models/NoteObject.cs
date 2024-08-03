using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updog.Core.Models;

public sealed class NoteObject
{
    public string Text { get; set; } = null!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
