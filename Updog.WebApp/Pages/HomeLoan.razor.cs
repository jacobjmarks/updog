using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Components.Chart.Models;
using Updog.Core;
using Updog.Core.Models;
using Updog.WebApp.Components;
using Updog.WebApp.Services;

namespace Updog.WebApp.Pages;

public partial class HomeLoan
{
    [CascadingParameter]
    public AppState AppState { get; set; } = null!;

    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    private AccountResource _homeLoanAccount = null!;
    private List<TransactionResource> _homeLoanTransactions = [];
    private string _totalDrawdown = null!;
    private string _totalInterest = null!;
    private string _totalRepayments = null!;
    private string _totalOffset = null!;
    private bool _loading = true;

    private List<TimeSeriesChartSeries> _balanceChartSeries = [];
    private ChartOptions _balanceChartOptions = new() { YAxisFormat = "C", ShowLegend = false };
    private List<TimeSeriesChartSeries> _interestChartSeries = [];
    private ChartOptions _interestChartOptions = new() { YAxisFormat = "C", ShowLegend = false };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await AppState.EnsureReadyAsync();

        if (AppState.UserIsLoggedIn && firstRender)
            await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        _loading = true;
        try
        {
            var up = AppState.UpBankApiClient;
            var accounts = (await up.GetAccountsAsync(pageSize: 100)).Data;

            var homeLoanAccount = accounts.FirstOrDefault(a => a.Attributes.AccountType == "HOME_LOAN");
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
            _homeLoanTransactions.Reverse();

            var totalDrawdown = _homeLoanTransactions
                .Where(t => t.Attributes.Amount.ValueInBaseUnits < 0
                    && t.Attributes.TransactionType != "Interest")
                .Sum(t => t.Attributes.Amount.ValueInBaseUnits) / 100m;
            _totalDrawdown = totalDrawdown.ToString("C");

            var totalRepayments = _homeLoanTransactions
                .Where(t => t.Attributes.Amount.ValueInBaseUnits > 0)
                .Sum(t => t.Attributes.Amount.ValueInBaseUnits) / 100m;
            _totalRepayments = totalRepayments.ToString("C");

            var totalInterest = _homeLoanTransactions
                .Where(t => t.Attributes.TransactionType == "Interest")
                .Sum(t => t.Attributes.Amount.ValueInBaseUnits) / 100m;
            _totalInterest = totalInterest.ToString("C");

            // Chart: Balance
            _balanceChartSeries.Add(new()
            {
                Data = _homeLoanTransactions
                    // .OrderBy(t => t.Attributes.SettledAt ?? t.Attributes.CreatedAt)
                    .Aggregate(new List<(DateTimeOffset Date, decimal Value)>(), (results, t) =>
                    {
                        var date = t.Attributes.SettledAt ?? t.Attributes.CreatedAt;

                        if (t.Attributes.TransactionType == "Drawdown"
                        || t.Attributes.TransactionType == "Lenders Mortgage Insurance")
                        {
                            if (results.Count == 0)
                                results.Add(new(date, -t.Attributes.Amount.ValueInBaseUnits));
                            else
                                results[0] = new(date, results[0].Value + -t.Attributes.Amount.ValueInBaseUnits);
                        }
                        else
                        {
                            var value = results.Count == 0
                                ? -t.Attributes.Amount.ValueInBaseUnits
                                : results.Last().Value + -t.Attributes.Amount.ValueInBaseUnits;
                            results.Add((date, value));
                        }

                        return results;
                    })
                    .Select(d => new TimeSeriesChartSeries.TimeValue(d.Date.DateTime, decimal.ToDouble(d.Value / 100m)))
                    .ToList(),
            });

            // Chart: Interest
            _interestChartSeries.Add(new());
            _interestChartSeries.Add(new());
            _interestChartSeries.Add(new()
            {
                Data = _homeLoanTransactions
                    .Where(t => t.Attributes.TransactionType == "Interest")
                    // .OrderBy(t => t.Attributes.SettledAt ?? t.Attributes.CreatedAt)
                    .Aggregate(new List<(DateTimeOffset Date, decimal Value)>(), (results, t) =>
                    {
                        var date = t.Attributes.SettledAt ?? t.Attributes.CreatedAt;
                        var value = -t.Attributes.Amount.ValueInBaseUnits;
                        results.Add((date, value));
                        return results;
                    })
                    .Select(d => new TimeSeriesChartSeries.TimeValue(d.Date.DateTime, decimal.ToDouble(d.Value / 100m)))
                    .ToList(),
            });
        }
        finally
        {
            _loading = false;
        }

        StateHasChanged();
    }
}