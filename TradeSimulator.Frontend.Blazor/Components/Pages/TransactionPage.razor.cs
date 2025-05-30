using Microsoft.AspNetCore.Components;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class TransactionPageBase : ComponentBase, IAsyncDisposable
    {
        [Parameter] public string BrokerId { get; set; }

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] TradeService TradeService { get; set; }


        protected List<Transaction> Transactions;
        protected bool IsLoading = false;



        /* --------------------------------------------------- */

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("OnInitializedAsync");
            IsLoading = true;

            if (!TradeService.IsConnected)
                Navigation.NavigateTo("/");

            Transactions = await TradeService.GetTransactions(BrokerId);

            await TradeService.OpenTransactionHistory();

            IsLoading = false;
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("DisposeAsync");
            await TradeService.CloseTransactionHistory();
        }
    }
}
