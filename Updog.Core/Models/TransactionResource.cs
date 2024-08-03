using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updog.Core.Models;

public sealed class TransactionResource : Resource
{
    public TransactionAttributes Attributes { get; set; } = null!;
}

public sealed class TransactionAttributes
{
    /// <summary>HELD, SETTLED</summary>
    public string Status { get; set; } = null!;
    public string? RawText { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Message { get; set; } = null!;
    public MoneyObject Amount { get; set; } = null!;
    public MoneyObject? ForeignAmount { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SettledAt { get; set; }
    public NoteObject? Note { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
