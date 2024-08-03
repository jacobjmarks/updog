using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Login
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private StateManager StateManager { get; set; } = null!;

    public string InputValue { get; set; } = null!;
    public bool RememberMe { get; set; }

    private bool _loggingIn = false;
    private MudForm loginForm = null!;
    private MudTextField<string> textField = null!;

    public async Task OnLoginButtonClicked()
    {
        await loginForm.Validate();
        if (!loginForm.IsValid)
            return;

        _loggingIn = true;

        try
        {
            if (!await StateManager.LoginAsync(InputValue, RememberMe))
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && await StateManager.IsAuthenticatedAsync())
            NavigationManager.NavigateTo("");
    }
}
