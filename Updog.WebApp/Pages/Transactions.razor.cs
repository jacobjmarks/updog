using Microsoft.AspNetCore.Components;
using Updog.Core.Models;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class Transactions
{
    [Inject] private StateManager StateManager { get; set; } = null!;

    public string? PrevPageLink { get; set; }
    public string? NextPageLink { get; set; }

    private IEnumerable<TransactionResource> _transactions = [];
    private bool _loading = true;
    private int _resultsPerPage = 25;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (await StateManager.EnsureAuthenticatedAsync() && firstRender)
        {
            await GetFirstPageAsync(notifyStateChanged: true);
        }
    }

    private async Task OnResultsPerPageChanged(int value)
    {
        _resultsPerPage = value;
        await GetFirstPageAsync();
    }

    private async Task GetFirstPageAsync(bool notifyStateChanged = false, CancellationToken ct = default)
    {
        _loading = true;
        try
        {
            var up = await StateManager.GetUpBankApiClientAsync();
            var page = await up.GetTransactionsAsync(
                    pageSize: _resultsPerPage,
                    ct: ct);

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
            var up = await StateManager.GetUpBankApiClientAsync();
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
            var up = await StateManager.GetUpBankApiClientAsync();
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
