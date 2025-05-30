using Microsoft.AspNetCore.Components;
using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;



namespace TradeSimulator.Backend.Components.Pages
{
    public class HomeBase : ComponentBase, IDisposable
    {
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] TradeService TradeService { get; set; }

        protected List<string> Messages = new();



        /* -------------------------------------------------------------------- */

        protected override async Task OnInitializedAsync()
        {
            if (!TradeService.IsConnected)
                await TradeService.Connect(Path.Combine(Navigation.BaseUri, "trade-hub"), "Server");

            TradeService.OnConnected += TradeService_OnConnected;
            TradeService.OnDisconnected += TradeService_OnDisconnected;

            TradeService.OnCreatedOrderBook += TradeService_OnCreateOrderBook;
            TradeService.OnDeletedOrderBook += TradeService_OnDeleteOrderBook;
            TradeService.OnOpenedOrderBook += TradeService_OnOpenedOrderBook;
            TradeService.OnClosedOrderBook += TradeService_OnClosedOrderBook;

            TradeService.OnOpenedTransactionHistory += TradeService_OnOpenedTransactionHistory;
            TradeService.OnClosedTransactionHistory += TradeService_OnClosedTransactionHistory;
        }

        public void Dispose()
        {
            TradeService.OnConnected -= TradeService_OnConnected;
            TradeService.OnDisconnected -= TradeService_OnDisconnected;

            TradeService.OnCreatedOrderBook -= TradeService_OnCreateOrderBook;
            TradeService.OnDeletedOrderBook -= TradeService_OnDeleteOrderBook;
            TradeService.OnOpenedOrderBook += TradeService_OnOpenedOrderBook;

            TradeService.OnOpenedTransactionHistory += TradeService_OnOpenedTransactionHistory;
        }



        /* -------------------------------------------------------------------- */

        private void AddMessage(string message)
        {
            Messages.Add(message);

            InvokeAsync(StateHasChanged);
        }



        /* -------------------------------------------------------------------- */

        private void TradeService_OnDeleteOrderBook(string username, OrderBook orderBook)
        {
            AddMessage($"{username}: Has deleted an order book ({orderBook.TickerId}).");
        }

        private void TradeService_OnCreateOrderBook(string username, OrderBook orderBook)
        {
            AddMessage($"{username}: Has created an order book ({orderBook.TickerId}).");
        }

        private void TradeService_OnOpenedOrderBook(string username, OrderBook orderBook)
        {
            AddMessage($"{username}: Has opened an order book ({orderBook.TickerId}).");
        }

        private void TradeService_OnClosedOrderBook(string username, OrderBook orderBook)
        {
            AddMessage($"{username}: Has closed an order book ({orderBook.TickerId}).");
        }

        private void TradeService_OnDisconnected(string username)
        {
            AddMessage($"{username}: Disconnected.");
        }

        private void TradeService_OnConnected(string username)
        {
            AddMessage($"{username}: Connected.");
        }

        private void TradeService_OnOpenedTransactionHistory(string username)
        {
            AddMessage($"{username}: Has opened the transaction history.");
        }

        private void TradeService_OnClosedTransactionHistory(string username)
        {
            AddMessage($"{username}: Has closed the transaction history.");
        }
    }
}
