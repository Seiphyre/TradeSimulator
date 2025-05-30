using Microsoft.AspNetCore.Components;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class OrderBookPageBase : ComponentBase, IAsyncDisposable
    {
        [Parameter] public string OrderBookId { get; set; }

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] TradeService TradeService { get; set; }


        protected OrderBook OrderBook;
        protected Ticker Ticker;
        protected bool IsLoading = false;



        /* ---------------------------------------------------------- */

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            if (!TradeService.IsConnected)
                Navigation.NavigateTo("/");

            OrderBook = await TradeService.GetOrderBook(OrderBookId);
            Ticker = await TradeService.GetTickerById(OrderBook.TickerId);

            await TradeService.OpenOrderBook(OrderBook.Id);

            IsLoading = false;
        }

        public async ValueTask DisposeAsync()
        {
            if (OrderBook is not null)
                await TradeService.CloseOrderBook(OrderBook.Id);
        }
    }
}
