using System.Globalization;
using System.Transactions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class HomeLoan
{
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public StateManager StateManager { get; set; } = null!;

    private AccountResource _homeLoanAccount = null!;
    private List<TransactionResource> _homeLoanTransactions = [];
    private string _totalDrawdown = null!;
    private string _totalInterest = null!;
    private string _totalRepayments = null!;
    private bool _loading = true;

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

            var getHomeLoanAccounts = await up.GetAccountsAsync(filterAccountType: "HOME_LOAN");
            var homeLoanAccount = getHomeLoanAccounts.Data.FirstOrDefault();
            if (homeLoanAccount == null)
            {
                NavigationManager.NavigateTo("");
                return;
            }
            _homeLoanAccount = homeLoanAccount;

            await foreach (var transaction in up.GetAllTransactionsByAccountAsync(homeLoanAccount.Id))
            {
                _homeLoanTransactions.Add(transaction);
            }

            var totalDrawdown = _homeLoanTransactions
                .Where(t => t.Attributes.Amount.ValueInBaseUnits < 0
                    && t.Attributes.TransactionType != "Interest")
                .Sum(t => t.Attributes.Amount.ValueInBaseUnits) / 100m;
            _totalDrawdown = totalDrawdown.ToString("C", new CultureInfo("en-AU"));

            var totalRepayments = _homeLoanTransactions
                .Where(t => t.Attributes.Amount.ValueInBaseUnits > 0)
                .Sum(t => t.Attributes.Amount.ValueInBaseUnits) / 100m;
            _totalRepayments = totalRepayments.ToString("C", new CultureInfo("en-AU"));

            var totalInterest = _homeLoanTransactions
                .Where(t => t.Attributes.TransactionType == "Interest")
                .Sum(t => t.Attributes.Amount.ValueInBaseUnits) / 100m;
            _totalInterest = totalInterest.ToString("C", new CultureInfo("en-AU"));
        }
        finally
        {
            _loading = false;
        }

        StateHasChanged();
    }
}