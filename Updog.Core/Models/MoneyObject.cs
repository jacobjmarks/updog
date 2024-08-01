using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updog.Core.Models;

public sealed class MoneyObject
{
    public string CurrencyCode { get; set; } = null!;
    public string Value { get; set; } = null!;
    public int ValueInBaseUnits { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public static class MoneyObjectExtensions
{
    public static string ToDisplayString(this MoneyObject @this)
    {
        var cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture)
            .Where(c =>
            {
                try
                {
                    return new RegionInfo(c.Name).ISOCurrencySymbol == @this.CurrencyCode;
                }
                catch
                {
                    return false;
                }
            })
            .FirstOrDefault();

        if (decimal.TryParse(@this.Value, NumberStyles.Currency, cultureInfo, out var decimalValue))
            return decimalValue.ToString("C", cultureInfo);
        else
            return $"{@this.CurrencyCode}{@this.Value}";
    }
}