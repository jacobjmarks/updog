using Microsoft.AspNetCore.Components;
using Updog.Core;

namespace Updog.WebApp.Services;

public sealed class AuthenticationService
{
    private readonly LocalStorageService _localStorageService;
    private readonly UpBankApiClientProvider _upBankApiClientProvider;
    private readonly NavigationManager _navigationManager;

    public AuthenticationService(
        LocalStorageService localStorageService,
        UpBankApiClientProvider upBankApiClientProvider,
        NavigationManager navigationManager)
    {
        _localStorageService = localStorageService;
        _upBankApiClientProvider = upBankApiClientProvider;
        _navigationManager = navigationManager;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var upApiKey = await _localStorageService.GetItemAsync<string?>("api-key");
            return !string.IsNullOrWhiteSpace(upApiKey);
        }
        catch
        {
            return false;
        }
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
        using var up = new UpBankApiClient(apiKey);
        if (!await up.PingAsync())
            return false;
        await _localStorageService.SetItemAsync("api-key", apiKey);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorageService.RemoveItemAsync("api-key");
        _navigationManager.NavigateTo("login");
    }
}
