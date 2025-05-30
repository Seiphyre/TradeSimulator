using Microsoft.AspNetCore.Components;
using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class TransactionPageBase : ComponentBase
    {
        [Parameter]
        public string BrokerId { get; set; }

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] TradeService TradeService { get; set; }


        protected List<Transaction> _transactions;
        protected bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (!TradeService.IsConnected)
                Navigation.NavigateTo("/");

            _transactions = await TradeService.GetTransactions(BrokerId);

            _isLoading = false;
        }
    }
}
