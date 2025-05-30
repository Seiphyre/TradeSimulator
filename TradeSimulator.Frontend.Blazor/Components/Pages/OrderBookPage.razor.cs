using Microsoft.AspNetCore.Components;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class OrderBookPageBase : ComponentBase
    {
        [Parameter] public string TickerId { get; set; }

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] TradeService TradeService { get; set; }


        protected Ticker Ticker;
        protected bool IsLoading = false;



        /* ---------------------------------------------------------- */

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            if (!TradeService.IsConnected)
                Navigation.NavigateTo("/");

            Ticker = await TradeService.GetTickerById(TickerId);

            IsLoading = false;
        }
    }
}
