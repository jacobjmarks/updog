using Microsoft.AspNetCore.Components;
using Updog.WebApp.Services;

namespace Updog.WebApp.Components;

public partial class EnsureAuthenticated : ComponentBase
{
    [Inject] private StateManager StateManager { get; set; } = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private bool _isAuthenticated = false;

    protected override async Task OnInitializedAsync()
    {
        _isAuthenticated = await StateManager.IsAuthenticatedAsync();
    }
}
