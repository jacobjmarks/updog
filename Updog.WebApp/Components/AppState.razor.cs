using Microsoft.AspNetCore.Components;
using Updog.Core;
using Updog.WebApp.Services;

namespace Updog.WebApp.Components;

public sealed partial class AppState : IDisposable
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public bool IsReady { get; private set; } = false;
    public bool UserIsLoggedIn { get; private set; } = false;
    public UpBankApiClient UpBankApiClient { get; private set; } = null!;
    public bool UserHasHomeLoanAccount { get; private set; } = false;

    [Inject] private SessionStorageService SessionStorageService { get; set; } = null!;
    [Inject] private LocalStorageService LocalStorageService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var apiKey = await LocalStorageService.GetItemAsync<string>("api-key")
            ?? await SessionStorageService.GetItemAsync<string>("api-key");

        if (!string.IsNullOrEmpty(apiKey))
            await LoginInternalAsync(apiKey);

        IsReady = true;
    }

    public async ValueTask EnsureReadyAsync()
    {
        if (!IsReady)
            await Task.Delay(50);
    }

    public async Task<bool> LoginAsync(string apiKey, bool rememberMe)
    {
        if (!await UpBankApiClient.PingAsync(apiKey))
            return false;

        if (rememberMe)
            await LocalStorageService.SetItemAsync("api-key", apiKey);
        else
            await SessionStorageService.SetItemAsync("api-key", apiKey);

        await LoginInternalAsync(apiKey);
        return true;
    }

    public async Task LogoutAsync()
    {
        await LocalStorageService.RemoveItemAsync("api-key");
        await SessionStorageService.RemoveItemAsync("api-key");

        UserIsLoggedIn = false;
        UpBankApiClient = null!;
        UserHasHomeLoanAccount = false;

        NavigationManager.NavigateTo("login");
    }

    private async Task LoginInternalAsync(string apiKey)
    {
        UserIsLoggedIn = true;
        UpBankApiClient = new(apiKey);
        UserHasHomeLoanAccount = (await UpBankApiClient.GetAccountsAsync(filterAccountType: "HOME_LOAN")).Data.Any();
    }

    public void Dispose()
    {
        UpBankApiClient?.Dispose();
    }
}
