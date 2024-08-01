using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public sealed partial class Home
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private UpBankApiClient Up { get; set; } = null!;
    [Inject] private ClipboardService ClipboardService { get; set; } = null!;
    [Inject] private LocalStorageService LocalStorageService { get; set; } = null!;

    private readonly HttpClient _httpClient = new();

    private Dictionary<string, List<AccountResource>>? _accountsByType;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            GetAccountsAsync().CatchAndLog();
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetAccountsAsync()
    {
        _accountsByType = new();

        await foreach (var account in Up.GetAllAccountsAsync())
        {
            if (!_accountsByType.TryGetValue(account.Attributes.AccountType, out var accounts))
                _accountsByType[account.Attributes.AccountType] = [account];
            else
                accounts.Add(account);
        }

        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
    }
}
