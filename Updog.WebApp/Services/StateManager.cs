using Microsoft.AspNetCore.Components;
using Updog.Core;

namespace Updog.WebApp.Services;

public sealed class StateManager(
    NavigationManager navigationManager,
    LocalStorageService localStorageService,
    SessionStorageService sessionStorageService)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly LocalStorageService _localStorageService = localStorageService;
    private readonly SessionStorageService _sessionStorageService = sessionStorageService;

    public UpBankApiClient UpBankApiClient { get; set; } = null!;

    private async Task<string?> GetUpBankApiKeyAsync()
    {
        return await _sessionStorageService.GetItemAsync<string>("api-key");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var apiKey = await GetUpBankApiKeyAsync();
        return apiKey != null;
    }

    public async Task<bool> EnsureAuthenticatedAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (!isAuthenticated)
            _navigationManager.NavigateTo("login");
        return isAuthenticated;
    }

    public async Task<bool> LoginAsync(string apiKey)
    {
        if (!await UpBankApiClient.PingAsync(apiKey))
            return false;
        await _sessionStorageService.SetItemAsync("api-key", apiKey);
        return true;
    }

    public async Task LogoutAsync()
    {
        await ClearStateAsync();
        _navigationManager.NavigateTo("login");
    }

    public async Task<UpBankApiClient> GetUpBankApiClientAsync()
    {
        var apiKey = await GetUpBankApiKeyAsync()
            ?? throw new InvalidOperationException();
        return new(apiKey);
    }

    private async Task ClearStateAsync()
    {
        await _sessionStorageService.RemoveItemAsync("api-key");
    }
}
