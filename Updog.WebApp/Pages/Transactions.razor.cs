using Microsoft.AspNetCore.Components;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Components;

namespace Updog.WebApp.Pages;

public partial class Transactions
{
    [CascadingParameter]
    public AppState AppState { get; set; } = null!;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    public string? PrevPageLink { get; set; }
    public string? NextPageLink { get; set; }

    private IEnumerable<TransactionResource> _transactions = [];
    private IEnumerable<AccountResource> _accounts = [];
    private bool _loading = true;
    private string? _filterByAccountId = null;
    [SupplyParameterFromQuery(Name = "account")]
    private string? FilterByAccountId
    {
        get => _filterByAccountId;
        set
        {
            if (value != _filterByAccountId)
            {
                _filterByAccountId = value;
                NavigationManager.NavigateTo(
                    NavigationManager.GetUriWithQueryParameter("account", value),
                    replace: true);
            }
        }
    }
    private int _resultsPerPage = 25;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await AppState.EnsureReadyAsync();

        if (AppState.UserIsLoggedIn && firstRender)
        {
            await GetAccountsAsync();
            await GetFirstPageAsync(notifyStateChanged: true);
        }
    }

    private async Task OnAccountFilterChanged(string? account)
    {
        FilterByAccountId = account;
        await GetFirstPageAsync();
    }

    private async Task OnResultsPerPageChanged(int value)
    {
        _resultsPerPage = value;
        await GetFirstPageAsync();
    }

    private async Task GetAccountsAsync()
    {
        var up = AppState.UpBankApiClient;
        var accounts = new List<AccountResource>();
        await foreach (var account in up.GetAllAccountsAsync())
            accounts.Add(account);
        _accounts = accounts;
        StateHasChanged();
    }

    private async Task GetFirstPageAsync(bool notifyStateChanged = false, CancellationToken ct = default)
    {
        _loading = true;
        try
        {
            var up = AppState.UpBankApiClient;
            PagedResource<TransactionResource> page;
            if (!string.IsNullOrEmpty(FilterByAccountId))
            {
                page = await up.GetTransactionsByAccountAsync(
                    FilterByAccountId,
                    pageSize: _resultsPerPage,
                    ct: ct);
            }
            else
            {
                page = await up.GetTransactionsAsync(
                    pageSize: _resultsPerPage,
                    ct: ct);
            }

            PrevPageLink = page.Links.Prev;
            NextPageLink = page.Links.Next;

            _transactions = page.Data;
        }
        finally
        {
            _loading = false;
        }

        if (notifyStateChanged)
            StateHasChanged();
    }

    public async Task OnFirstButtonClicked()
    {
        await GetFirstPageAsync();
    }

    public async Task OnPrevButtonClicked()
    {
        if (PrevPageLink == null)
            return;

        _loading = true;
        try
        {
            var up = AppState.UpBankApiClient;
            var page = await up.GetAsync<PagedResource<TransactionResource>>(PrevPageLink);

            PrevPageLink = page.Links.Prev;
            NextPageLink = page.Links.Next;

            _transactions = page.Data;
        }
        finally
        {
            _loading = false;
        }
    }

    public async Task OnNextButtonClicked()
    {
        if (NextPageLink == null)
            return;

        _loading = true;
        try
        {
            var up = AppState.UpBankApiClient;
            var page = await up.GetAsync<PagedResource<TransactionResource>>(NextPageLink);

            PrevPageLink = page.Links.Prev;
            NextPageLink = page.Links.Next;

            _transactions = page.Data;
        }
        finally
        {
            _loading = false;
        }
    }
}
