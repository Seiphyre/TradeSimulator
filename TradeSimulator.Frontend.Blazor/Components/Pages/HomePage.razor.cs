using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TradeSimulator.Frontend.Blazor.Components.Components;
using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class HomePageBase : ComponentBase, IDisposable
    {

        private readonly string _defaultHubURL = "http://localhost:5038/trade-hub";

        // --

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] TradeService TradeService { get; set; }

        // --

        public bool IsConnected
        { 
            get { return TradeService.IsConnected; } 
        }

        private bool _isConnecting = false;
        public bool IsConnecting
        { 
            get { return _isConnecting; } 
        }

        private bool _isDisconnecting = false;
        public bool IsDisconnecting 
        { 
            get { return _isDisconnecting; } 
        }

        // --

        protected string BrokerId;
        protected Broker Broker;
        protected List<Ticker> Tickers;
        protected List<OrderBook> OrderBooks;
        protected List<Transaction> Transactions;

        protected OrderBook HoveredOrderBook = null;



        /* ---------------------------------------------------------- */

        protected override void OnInitialized()
        {
            BrokerId = GenerateRandomBrokerId();

            TradeService.OnCreateOrderBook += TradeHub_OnCreateOrderBook;
            TradeService.OnDeleteOrderBook += TradeHub_OnDeleteOrderBook;
        }

        // --

        public void Dispose()
        {
            TradeService.OnCreateOrderBook -= TradeHub_OnCreateOrderBook;
            TradeService.OnDeleteOrderBook -= TradeHub_OnDeleteOrderBook;
        }



        /* ---------------------------------------------------------- */

        protected async Task Connect()
        {
            Console.WriteLine("Connecting...");

            _isConnecting = true;
            StateHasChanged();

            try
            {
                await TradeService.Connect(_defaultHubURL, BrokerId);

                Console.WriteLine("Connected !");
                Snackbar.Add($"Successfully Connected to hub", Severity.Success);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to connect.");
                Snackbar.Add($"Failed to connect. {exception.Message}", Severity.Error);
            }
            finally
            {
                _isConnecting = false;
                StateHasChanged();
            }
        }

        protected async Task Disconnect()
        {
            Console.WriteLine("Disconnecting...");

            _isDisconnecting = true;
            StateHasChanged();

            try
            {
                await TradeService.Disconnect();

                Console.WriteLine("Disconnected.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to disconnect.");
                Snackbar.Add($"Failed to disconnect. {exception.Message}", Severity.Error);
            }
            finally
            {
                _isDisconnecting = false;
                StateHasChanged();
            }
        }



        /* ---------------------------------------------------------- */

        protected string GenerateRandomBrokerId()
        {
            return "Victor";
        }

        protected async Task GetOrCreateBroker()
        {
            Broker = await TradeService.GetOrCreateBroker(BrokerId);
        }



        /* ---------------------------------------------------------- */

        protected async Task GetOrderBooks()
        {
            OrderBooks = await TradeService.GetOrderBooks(BrokerId);
        }

        // --

        protected async Task<OrderBook> CreateOrderBook(string brokerId, string tickerId)
        {
            var newOrderBook = await TradeService.CreateOrderBook(BrokerId, tickerId);
            AddOrderBookToList(newOrderBook);

            return newOrderBook;
        }

        protected void AddOrderBookToList(OrderBook newOrderBook)
        {
            if (OrderBooks == null)
                OrderBooks = new List<OrderBook>();

            if (!OrderBooks.Any(orderBook => orderBook.Id == newOrderBook.Id))
            {
                OrderBooks.Add(newOrderBook);
                StateHasChanged();
            }
        }

        // --

        protected async Task DeleteOrderBook(string orderBookId)
        {
            await TradeService.DeleteOrderBook(orderBookId);
            RemoveOrderBookFromList(orderBookId);
        }

        protected void RemoveOrderBookFromList(string orderBookId)
        {
            if (OrderBooks == null)
                return;

            OrderBooks.RemoveAll(orderBook => orderBook.Id == orderBookId);
            StateHasChanged();
        }

        // --

        protected async Task ShowCreateOrderBookDialog()
        {
            var parameters = new DialogParameters<CreateOrderBookDialog>
            {
                { x => x.Tickers, Tickers },
            };

            var dialog = await DialogService.ShowAsync<CreateOrderBookDialog>(null, parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Ticker ticker = result.Data as Ticker;

                var newOrderBook = await CreateOrderBook(BrokerId, ticker.Id);

                await OpenOrderBookPage(newOrderBook);
            }
        }

        protected async Task OpenOrderBookPage(OrderBook orderBook)
        {
            string url = Path.Combine(Navigation.BaseUri, "order-book", orderBook.TickerId);

            await JSRuntime.InvokeVoidAsync("open", url, "_blank");
        }



        /* ---------------------------------------------------------- */

        protected async Task GetTickers()
        {
            Tickers = await TradeService.GetTickers();
        }



        /* ---------------------------------------------------------- */

        protected async Task GetTransactions()
        {
            Transactions = await TradeService.GetTransactions(BrokerId);
        }

        protected async Task OpenTransactionsPage()
        {
            string url = Path.Combine(Navigation.BaseUri, "transactions", BrokerId);

            await JSRuntime.InvokeVoidAsync("open", url, "_blank");
        }


        /* ---------------------------------------------------------- */

        protected async void ConnectBtn_OnClick()
        {
            await Connect();

            // --

            await GetOrCreateBroker();
            await GetTickers();
            await GetOrderBooks();
            await GetTransactions();

            StateHasChanged();
        }

        protected async void DisconnectBtn_OnClick()
        {
            await Disconnect();
        }

        // --

        protected async void TradeHub_OnCreateOrderBook(string username, OrderBook orderBook)
        {
            await InvokeAsync(() => 
            {
                if (orderBook.BrokerId == BrokerId)
                    AddOrderBookToList(orderBook);
            });
        }

        protected async void TradeHub_OnDeleteOrderBook(string username, OrderBook orderBook)
        {
            await InvokeAsync(() =>
            {
                if (orderBook.BrokerId == BrokerId)
                    RemoveOrderBookFromList(orderBook.Id);
            });
        }

        // --

        protected async void CreateOrderBookBtn_OnClick()
        {
            await ShowCreateOrderBookDialog();
        }

        protected void OrderBookTable_OnRowMouseEnter(TableRowHoverEventArgs<OrderBook> eventArgs)
        {
            HoveredOrderBook = eventArgs.Item;
        }

        protected void OrderBookTable_OnRowMouseLeave(TableRowHoverEventArgs<OrderBook> eventArgs)
        {
            HoveredOrderBook = null;
        }

        // --

        protected async void OpenTransactionBtn_OnClick()
        {
            await OpenTransactionsPage();
        }
    }
}
