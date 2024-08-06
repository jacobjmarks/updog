using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.WebApp.Components;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Login
{
    [CascadingParameter]
    public AppState AppState { get; set; } = null!;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    public string InputValue { get; set; } = null!;
    public bool RememberMe { get; set; }

    private bool _loggingIn = false;
    private MudForm loginForm = null!;
    private MudTextField<string> textField = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await AppState.EnsureReadyAsync();

        if (AppState.UserIsLoggedIn && firstRender)
            NavigationManager.NavigateTo("");
    }

    public async Task OnLoginButtonClicked()
    {
        await loginForm.Validate();
        if (!loginForm.IsValid)
            return;

        _loggingIn = true;

        try
        {
            if (!await AppState.LoginAsync(InputValue, RememberMe))
            {
                textField.Error = true;
                textField.ErrorText = "Invalid token";
            }
            else
            {
                NavigationManager.NavigateTo("");
            }
        }
        finally
        {
            _loggingIn = false;
        }
    }
}
