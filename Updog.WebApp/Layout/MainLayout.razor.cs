using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.WebApp.Components;
using Updog.WebApp.Services;

namespace Updog.WebApp.Layout;

public partial class MainLayout
{
    [CascadingParameter]
    public AppState AppState { get; set; } = null!;

    [Inject] private LocalStorageService LocalStorageService { get; set; } = null!;

    private MudThemeProvider _mudThemeProvider = null!;

    private bool _isDarkMode;
    private bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            _isDarkMode = value;
            LocalStorageService.SetItemAsync("updog:cfg:dark-mode", value).CatchAndLog();
        }
    }

    private string _darkLightModeText = null!;
    private string _darkLightModeIcon = null!;

    private bool _drawerOpen = true;

    protected override async Task OnInitializedAsync()
    {
        await AppState.EnsureReadyAsync();

        var themePreference = await LocalStorageService.GetItemAsync<bool?>("updog:cfg:dark-mode");
        IsDarkMode = themePreference ?? await _mudThemeProvider.GetSystemPreference();
        UpdateDarkLightModeIcon();
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
        await AppState.LogoutAsync();
    }
}
