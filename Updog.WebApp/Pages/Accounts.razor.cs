using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Accounts
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private UpBankApiClientProvider UpBankApiClientProvider { get; set; } = null!;
    [Inject] private ClipboardService ClipboardService { get; set; } = null!;
    [Inject] private LocalStorageService LocalStorageService { get; set; } = null!;
    [Inject] private AuthenticationService AuthenticationService { get; set; } = null!;

    private Dictionary<string, List<AccountResource>>? _accountsByType;

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine($"{nameof(Pages)}.{nameof(Accounts)}:{nameof(OnInitializedAsync)}");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Console.WriteLine($"{nameof(Pages)}.{nameof(Accounts)}:{nameof(OnAfterRenderAsync)}(firstRender: {firstRender})");
        if (await AuthenticationService.EnsureAuthenticatedAsync() && firstRender)
        {
            await GetAccountsAsync();
        }
    }

    private async Task GetAccountsAsync()
    {
        using var up = await UpBankApiClientProvider.GetClientAsync();

        _accountsByType = [];

        await foreach (var account in up.GetAllAccountsAsync())
        {
            if (!_accountsByType.TryGetValue(account.Attributes.AccountType, out var accounts))
                _accountsByType[account.Attributes.AccountType] = [account];
            else
                accounts.Add(account);
        }

        StateHasChanged();
    }
}
