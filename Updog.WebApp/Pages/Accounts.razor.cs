using Microsoft.AspNetCore.Components;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Accounts
{
    [Inject] private StateManager StateManager { get; set; } = null!;

    private bool _loading = true;
    private AccountResource _spendingAccount = null!;
    private AccountResource? _2UpSpendingAccount;
    private List<AccountResource> _savers = [];
    private List<AccountResource> _2UpSavers = [];
    private AccountResource? _homeLoanAccount;

    private Dictionary<string, List<AccountResource>>? _accountsByType;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Console.WriteLine($"{nameof(Pages)}.{nameof(Accounts)}:{nameof(OnAfterRenderAsync)}(firstRender: {firstRender})");
        if (await StateManager.EnsureAuthenticatedAsync() && firstRender)
        {
            await InitializeAsync();
        }
    }

    private async Task InitializeAsync()
    {
        _loading = true;
        try
        {
            var up = await StateManager.GetUpBankApiClientAsync();

            _accountsByType = [];

            await foreach (var account in up.GetAllAccountsAsync())
            {
                if (account.Attributes.AccountType == "TRANSACTIONAL")
                {
                    if (account.Attributes.OwnershipType == "JOINT")
                        _2UpSpendingAccount = account;
                    else
                        _spendingAccount = account;
                }
                else if (account.Attributes.AccountType == "SAVER")
                {
                    if (account.Attributes.OwnershipType == "JOINT")
                        _2UpSavers.Add(account);
                    else
                        _savers.Add(account);
                }
                else if (account.Attributes.AccountType == "HOME_LOAN")
                {
                    _homeLoanAccount = account;
                }
            }
        }
        finally
        {
            _loading = false;
        }

        StateHasChanged();
    }
}
