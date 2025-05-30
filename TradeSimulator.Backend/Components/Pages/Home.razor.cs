using Microsoft.AspNetCore.Components;

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
            TradeService.OnCreateOrderBook += TradeService_OnCreateOrderBook;
            TradeService.OnDeleteOrderBook += TradeService_OnDeleteOrderBook;
        }

        public void Dispose()
        {
            TradeService.OnConnected -= TradeService_OnConnected;
            TradeService.OnDisconnected -= TradeService_OnDisconnected;
            TradeService.OnCreateOrderBook -= TradeService_OnCreateOrderBook;
            TradeService.OnDeleteOrderBook -= TradeService_OnDeleteOrderBook;

        }



        /* -------------------------------------------------------------------- */

        private void AddMessage(string message)
        {
            Messages.Add(message);

            InvokeAsync(StateHasChanged);
        }



        /* -------------------------------------------------------------------- */

        private void TradeService_OnDeleteOrderBook(string username, Shared.Models.OrderBook orderBook)
        {
            AddMessage($"{username}: Has deleted an order book ({orderBook.TickerId}).");
        }

        private void TradeService_OnCreateOrderBook(string username, Shared.Models.OrderBook orderBook)
        {
            AddMessage($"{username}: Has created an order book ({orderBook.TickerId}).");
        }

        private void TradeService_OnDisconnected(string username)
        {
            Console.WriteLine("Disconnected");

            AddMessage($"{username}: Disconnected.");
        }

        private void TradeService_OnConnected(string username)
        {
            Console.WriteLine("Connected");

            AddMessage($"{username}: Connected.");
        }
    }
}
