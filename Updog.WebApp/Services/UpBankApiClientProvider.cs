using Updog.Core;

namespace Updog.WebApp.Services;

public sealed class UpBankApiClientProvider(LocalStorageService localStorage)
{
    private readonly LocalStorageService _localStorage = localStorage;

    public async Task<UpBankApiClient> GetClientAsync()
    {
        var apiKey = await _localStorage.GetItemAsync<string>("api-key")
        ?? throw new InvalidOperationException();
        return new(apiKey);
    }
}
