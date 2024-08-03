using Microsoft.AspNetCore.Components;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Transactions
{
    [Inject] private StateManager StateManager { get; set; } = null!;

    private List<TransactionResource> _transactions = [];
    private bool _loading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Console.WriteLine($"{nameof(Pages)}.{nameof(Accounts)}:{nameof(OnAfterRenderAsync)}(firstRender: {firstRender})");
        if (await StateManager.EnsureAuthenticatedAsync() && firstRender)
        {
            await GetTransactionsAsync();
        }
    }

    private async Task GetTransactionsAsync()
    {
        _loading = true;
        try
        {
            var up = await StateManager.GetUpBankApiClientAsync();

            var page = await up.GetTransactionsAsync();
            _transactions = page.Data.ToList();
        }
        finally
        {
            _loading = false;
        }

        StateHasChanged();
    }
}
