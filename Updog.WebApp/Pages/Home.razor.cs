using Microsoft.AspNetCore.Components;

namespace Updog.WebApp.Pages;

public partial class Home
{
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    protected override void OnInitialized()
    {
        NavigationManager.NavigateTo("/accounts", replace: true);
    }
}
