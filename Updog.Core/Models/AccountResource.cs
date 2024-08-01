using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updog.Core.Models;

public sealed class AccountResource : Resource
{
    public AccountAttributes Attributes { get; set; } = null!;
}

public sealed class AccountAttributes
{
    public string DisplayName { get; set; } = null!;
    public string AccountType { get; set; } = null!;
    public string OwnershipType { get; set; } = null!;
    public MoneyObject Balance { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
