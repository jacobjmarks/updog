using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Updog.Core.Models;

namespace Updog.Core;

public sealed class UpBankApiClient : IDisposable
{
    internal readonly HttpClient _httpClient = new();

    public UpBankApiClient(string apiKey)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.BaseAddress = new Uri("https://api.up.com.au");
    }

    public static async Task<bool> PingAsync(string apiKey, CancellationToken ct = default)
    {
        using var up = new UpBankApiClient(apiKey);
        return await up.PingAsync(ct);
    }

    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        using var response = await _httpClient.GetAsync("api/v1/util/ping", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<PagedResource<AccountResource>> GetAccountsAsync(CancellationToken ct = default)
    {
        using var response = await _httpClient.GetAsync("api/v1/accounts", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResource<AccountResource>>(cancellationToken: ct) ?? throw new JsonException();
    }

    public async Task<PagedResource<TransactionResource>> GetTransactionsAsync(
        int? pageSize = null,
        string? filterStatus = null,
        DateTimeOffset? filterSince = null,
        DateTimeOffset? filterUntil = null,
        string? filterCategory = null,
        string? filterTag = null,
        CancellationToken ct = default)
    {
        var qs = new QueryString();
        if (pageSize != null)
            qs = qs.Add("page[size]", pageSize.ToString());
        if (filterStatus != null)
            qs = qs.Add("filter[status]", filterStatus);
        if (filterSince.HasValue)
            qs = qs.Add("filter[since]", filterSince.Value.ToString("o"));
        if (filterUntil.HasValue)
            qs = qs.Add("filter[since]", filterUntil.Value.ToString("o"));
        if (filterCategory != null)
            qs = qs.Add("filter[category]", filterCategory);
        if (filterTag != null)
            qs = qs.Add("filter[tag]", filterTag);

        using var response = await _httpClient.GetAsync("api/v1/transactions" + qs, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResource<TransactionResource>>(cancellationToken: ct) ?? throw new JsonException();
    }

    public async Task<T> GetAsync<T>(string url, CancellationToken ct = default)
    {
        using var response = await _httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct) ?? throw new JsonException();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

public static class UpBankApiClientExtensions
{
    public static IAsyncEnumerable<AccountResource> GetAllAccountsAsync(this UpBankApiClient client, CancellationToken ct = default)
    {
        return Exhaust(client, (c, ct) => c.GetAccountsAsync(ct), ct);
    }

    public static IAsyncEnumerable<TransactionResource> GetAllTransactionsAsync(this UpBankApiClient client, CancellationToken ct = default)
    {
        return Exhaust(client, (c, ct) => c.GetTransactionsAsync(pageSize: 100, ct: ct), ct);
    }

    private static async IAsyncEnumerable<T> Exhaust<T>(UpBankApiClient client, Func<UpBankApiClient, CancellationToken, Task<PagedResource<T>>> getFirstPage, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var currentPage = await getFirstPage(client, ct);
        foreach (var resource in currentPage.Data)
        {
            ct.ThrowIfCancellationRequested();
            yield return resource;
        }

        var nextPageUrl = currentPage.Links.Next;

        while (nextPageUrl != null)
        {
            ct.ThrowIfCancellationRequested();
            using var response = await client._httpClient.GetAsync(nextPageUrl, ct);
            response.EnsureSuccessStatusCode();

            currentPage = await response.Content.ReadFromJsonAsync<PagedResource<T>>(cancellationToken: ct) ?? throw new JsonException();
            foreach (var resource in currentPage.Data)
            {
                ct.ThrowIfCancellationRequested();
                yield return resource;
            }

            nextPageUrl = currentPage.Links.Next;
        }
    }
}