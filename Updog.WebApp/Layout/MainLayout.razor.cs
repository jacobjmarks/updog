using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.WebApp.Services;

namespace Updog.WebApp.Layout;

public partial class MainLayout
{
    [Inject] private LocalStorageService LocalStorageService { get; set; } = null!;
    [Inject] private StateManager StateManager { get; set; } = null!;

    private MudThemeProvider _mudThemeProvider = null!;

    private bool _isDarkMode;
    private bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            _isDarkMode = value;
            LocalStorageService.SetItemAsync("qsd:cfg:dark-mode", value).CatchAndLog();
        }
    }

    private string _darkLightModeText = null!;
    private string _darkLightModeIcon = null!;

    private bool _drawerOpen = true;

    private bool _userIsLoggedIn = false;
    public bool UserIsLoggedIn
    {
        get => _userIsLoggedIn; set
        {
            var notifyStateChanged = value != _userIsLoggedIn;
            _userIsLoggedIn = value;
            if (notifyStateChanged)
                StateHasChanged();
        }
    }

    private bool? _userHasHomeLoanAccount;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Console.WriteLine($"{nameof(MainLayout)}:{nameof(OnAfterRenderAsync)}(firstRender: {firstRender})");
        UserIsLoggedIn = await StateManager.IsAuthenticatedAsync();

        if (firstRender)
        {
            var themePreference = await LocalStorageService.GetItemAsync<bool?>("qsd:cfg:dark-mode");
            IsDarkMode = themePreference ?? await _mudThemeProvider.GetSystemPreference();
            UpdateDarkLightModeIcon();

            StateHasChanged();
        }

        if (_userHasHomeLoanAccount == null && UserIsLoggedIn)
        {
            var up = await StateManager.GetUpBankApiClientAsync();
            _userHasHomeLoanAccount = (await up.GetAccountsAsync(filterAccountType: "HOME_LOAN")).Data.Any();
            StateHasChanged();
        }
    }

    private void OnMenuButtonClicked()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void ToggleDarkLightMode()
    {
        IsDarkMode = !IsDarkMode;
        UpdateDarkLightModeIcon();
    }

    private void UpdateDarkLightModeIcon()
    {
        if (IsDarkMode)
        {
            _darkLightModeText = "Switch to Light Mode";
            _darkLightModeIcon = Icons.Material.Filled.LightMode;
        }
        else
        {
            _darkLightModeText = "Switch to Dark Mode";
            _darkLightModeIcon = Icons.Material.Filled.DarkMode;
        }
    }

    private async Task OnLogoutButtonClicked()
    {
        await StateManager.LogoutAsync();
        UserIsLoggedIn = false;
    }
}
