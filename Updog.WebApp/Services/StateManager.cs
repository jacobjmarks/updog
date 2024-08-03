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

    private string? _upBankApiKey = null;

    private async Task<string?> GetUpBankApiKeyAsync()
    {
        _upBankApiKey ??= await _localStorageService.GetItemAsync<string>("api-key");
        _upBankApiKey ??= await _sessionStorageService.GetItemAsync<string>("api-key");
        return _upBankApiKey;
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

    public async Task<bool> LoginAsync(string apiKey, bool rememberMe)
    {
        if (!await UpBankApiClient.PingAsync(apiKey))
            return false;

        if (rememberMe)
            await _localStorageService.SetItemAsync("api-key", apiKey);
        else
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
        _upBankApiKey = null;
        await _localStorageService.RemoveItemAsync("api-key");
        await _sessionStorageService.RemoveItemAsync("api-key");
    }
}
