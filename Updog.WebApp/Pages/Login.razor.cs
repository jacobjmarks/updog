using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.Core;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Login
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthenticationService AuthenticationService { get; set; } = null!;

    public string InputValue { get; set; } = null!;
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
            if (!await AuthenticationService.LoginAsync(InputValue))
            {
                textField.Error = true;
                textField.ErrorText = "Invalid token";
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
        }
        finally
        {
            _loggingIn = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && await AuthenticationService.IsAuthenticatedAsync())
            NavigationManager.NavigateTo("/");
    }
}
