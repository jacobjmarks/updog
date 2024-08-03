using Microsoft.AspNetCore.Components;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Accounts
{
    [Inject] private StateManager StateManager { get; set; } = null!;

    private Dictionary<string, List<AccountResource>>? _accountsByType;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Console.WriteLine($"{nameof(Pages)}.{nameof(Accounts)}:{nameof(OnAfterRenderAsync)}(firstRender: {firstRender})");
        if (await StateManager.EnsureAuthenticatedAsync() && firstRender)
        {
            await GetAccountsAsync();
        }
    }

    private async Task GetAccountsAsync()
    {
        var up = await StateManager.GetUpBankApiClientAsync();

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
