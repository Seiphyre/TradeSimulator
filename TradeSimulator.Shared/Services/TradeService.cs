using System.Security.Claims;

using Microsoft.AspNetCore.SignalR.Client;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Utils;



namespace TradeSimulator.Shared.Services
{
    public delegate void OnConnected(string username);
    public delegate void OnDisconnected(string username);
    public delegate void OnCreatedOrderBook(string username, OrderBook orderBook);
    public delegate void OnDeletedOrderBook(string username, OrderBook orderBook);
    public delegate void OnOpenedOrderBook(string username, OrderBook orderBook);
    public delegate void OnClosedOrderBook(string username, OrderBook orderBook);
    public delegate void OnOpenedTransactionHistory(string username);
    public delegate void OnClosedTransactionHistory(string username);

    public class TradeService : ITradeHubClient, IAsyncDisposable
    {
        public event OnConnected OnConnected;
        public event OnDisconnected OnDisconnected;
        public event Action OnReconnecting;
        public event Action OnReconnected;

        public event OnCreatedOrderBook OnCreatedOrderBook;
        public event OnDeletedOrderBook OnDeletedOrderBook;
        public event OnOpenedOrderBook OnOpenedOrderBook;
        public event OnClosedOrderBook OnClosedOrderBook;

        public event OnOpenedTransactionHistory OnOpenedTransactionHistory;
        public event OnClosedTransactionHistory OnClosedTransactionHistory;



        private HubConnection _hubConnection;

        public bool IsConnected
        {
            get
            {
                return _hubConnection?.State == HubConnectionState.Connected;
            }
        }



        /* --------------------------------------------------------------- */

        public async Task Connect(string url, string username)
        {
            // -- Build connection

            string jwt = JWTUtils.WriteToken(new WriteTokenOptions()
            {
                SecretKey = "70686367d1ce87264121d74a5abec5eeae3f54c70f0204017b9ab758e69a7e8d",
                Claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, username)
                        },
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(jwt);
                    options.Headers.Add("Bearer", $"{jwt}"); // Needed for WPF app
                })
                .WithAutomaticReconnect()
                .Build();

            // -- Subscribe to hub messages 

            _hubConnection.On<string>(nameof(ITradeHubClient.Connected), Connected);
            _hubConnection.On<string>(nameof(ITradeHubClient.Disconnected), Disconnected);

            _hubConnection.On<string, OrderBook>(nameof(ITradeHubClient.CreatedOrderBook), CreatedOrderBook);
            _hubConnection.On<string, OrderBook>(nameof(ITradeHubClient.DeletedOrderBook), DeletedOrderBook);
            _hubConnection.On<string, OrderBook>(nameof(ITradeHubClient.OpenedOrderBook), OpenedOrderBook);
            _hubConnection.On<string, OrderBook>(nameof(ITradeHubClient.ClosedOrderBook), ClosedOrderBook);

            _hubConnection.On<string>(nameof(ITradeHubClient.OpenedTransactionHistory), OpenedTransactionHistory);
            _hubConnection.On<string>(nameof(ITradeHubClient.ClosedTransactionHistory), ClosedTransactionHistory);
            _hubConnection.Reconnecting += _hubConnection_Reconnecting;
            _hubConnection.Reconnected += _hubConnection_Reconnected;

            // -- Start connection

            await _hubConnection.StartAsync();
        }

        public async Task Disconnect()
        {
            await _hubConnection.StopAsync();
        }



        /* --------------------------------------------------------------- */

        public async Task<List<Ticker>> GetTickers()
        {
            return await _hubConnection.InvokeAsync<List<Ticker>>(nameof(ITradeHub.GetTickers));
        }

        public async Task<Ticker> GetTickerById(string tickerId)
        {
            return await _hubConnection.InvokeAsync<Ticker>(nameof(ITradeHub.GetTickerById), tickerId);
        }



        /* --------------------------------------------------------------- */

        public async Task<Broker> GetOrCreateBroker(string brokerId)
        {
            return await _hubConnection.InvokeAsync<Broker>(nameof(ITradeHub.GetOrCreateBroker), brokerId);
        }



        /* --------------------------------------------------------------- */

        public async Task<OrderBook> GetOrderBook(string orderBookId)
        {
            return await _hubConnection.InvokeAsync<OrderBook>(nameof(ITradeHub.GetOrderBook), orderBookId);
        }

        public async Task<List<OrderBook>> GetOrderBooks(string brokerId = null)
        {
            return await _hubConnection.InvokeAsync<List<OrderBook>>(nameof(ITradeHub.GetOrderBooks), brokerId);
        }

        public async Task<OrderBook> CreateOrderBook(string brokerId, string tickerId)
        {
            return await _hubConnection.InvokeAsync<OrderBook>(nameof(ITradeHub.CreateOrderBook), brokerId, tickerId);
        }

        public async Task DeleteOrderBook(string orderBookId)
        {
            await _hubConnection.InvokeAsync(nameof(ITradeHub.DeleteOrderBook), orderBookId);
        }

        public async Task OpenOrderBook(string orderBookId)
        {
            await _hubConnection.InvokeAsync(nameof(ITradeHub.OpenOrderBook), orderBookId);
        }

        public async Task CloseOrderBook(string orderBookId)
        {
            await _hubConnection.InvokeAsync(nameof(ITradeHub.CloseOrderBook), orderBookId);
        }



        /* --------------------------------------------------------------- */

        public async Task<List<Transaction>> GetTransactions(string brokerId = null)
        {
            return await _hubConnection.InvokeAsync<List<Transaction>>(nameof(ITradeHub.GetTransactions), brokerId);
        }

        public async Task OpenTransactionHistory()
        {
            await _hubConnection.InvokeAsync(nameof(ITradeHub.OpenTransactionHistory));
        }

        public async Task CloseTransactionHistory()
        {
            await _hubConnection.InvokeAsync(nameof(ITradeHub.CloseTransactionHistory));
        }



        /* --------------------------------------------------------------- */

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }



        /* --------------------------------------------------------------- */

        public Task Connected(string username)
        {
            OnConnected?.Invoke(username);

            return Task.CompletedTask;
        }

        public Task Disconnected(string username)
        {
            OnDisconnected?.Invoke(username);

            return Task.CompletedTask;
        }

        private Task _hubConnection_Reconnected(string arg)
        {
            OnReconnected?.Invoke();

            return Task.CompletedTask;
        }

        private Task _hubConnection_Reconnecting(Exception exception)
        {
            OnReconnecting?.Invoke();

            return Task.CompletedTask;
        }

        public Task CreatedOrderBook(string username, OrderBook orderBook)
        {
            OnCreatedOrderBook?.Invoke(username, orderBook);

            return Task.CompletedTask;
        }

        public Task DeletedOrderBook(string username, OrderBook orderBook)
        {
            OnDeletedOrderBook?.Invoke(username, orderBook);

            return Task.CompletedTask;
        }

        public Task OpenedOrderBook(string username, OrderBook orderBook)
        {
            OnOpenedOrderBook?.Invoke(username, orderBook);

            return Task.CompletedTask;
        }

        public Task ClosedOrderBook(string username, OrderBook orderBook)
        {
            OnClosedOrderBook?.Invoke(username, orderBook);

            return Task.CompletedTask;
        }

        public Task OpenedTransactionHistory(string username)
        {
            OnOpenedTransactionHistory?.Invoke(username);

            return Task.CompletedTask;
        }

        public Task ClosedTransactionHistory(string username)
        {
            OnClosedTransactionHistory?.Invoke(username);

            return Task.CompletedTask;
        }



        /* --------------------------------------------------------------- */
    }
}
