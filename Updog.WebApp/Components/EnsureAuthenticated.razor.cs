using Microsoft.AspNetCore.Components;

namespace Updog.WebApp.Components;

public partial class EnsureAuthenticated
{
    [CascadingParameter]
    public AppState AppState { get; set; } = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private bool _isAuthenticated;

    protected override async Task OnParametersSetAsync()
    {
        await AppState.EnsureReadyAsync();

        _isAuthenticated = AppState.UserIsLoggedIn;
        if (!_isAuthenticated)
            NavigationManager.NavigateTo("login");
    }
}
