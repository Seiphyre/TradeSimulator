using System.Security.Claims;

using Microsoft.AspNetCore.SignalR.Client;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Utils;



namespace TradeSimulator.Shared.Services
{
    public delegate void OnConnected(string username);
    public delegate void OnDisconnected(string username);
    public delegate void OnCreateOrderBook(string username, OrderBook orderBook);
    public delegate void OnDeleteOrderBook(string username, OrderBook orderBook);

    public class TradeService : ITradeHubClient, IAsyncDisposable
    {
        public event OnConnected OnConnected;
        public event OnDisconnected OnDisconnected;

        public event OnCreateOrderBook OnCreateOrderBook;
        public event OnDeleteOrderBook OnDeleteOrderBook;

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

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(JWTUtils.WriteToken(new WriteTokenOptions()
                    {
                        SecretKey = "70686367d1ce87264121d74a5abec5eeae3f54c70f0204017b9ab758e69a7e8d",
                        Claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, username)
                        },
                        ExpirationDate = DateTime.Now.AddHours(1)
                    }));
                })
                .WithAutomaticReconnect()
                .Build();

            // -- Subscribe to hub messages 

            _hubConnection.On<string, OrderBook>(nameof(ITradeHubClient.CreatedOrderBook), CreatedOrderBook);
            _hubConnection.On<string, OrderBook>(nameof(ITradeHubClient.DeletedOrderBook), DeletedOrderBook);

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



        /* --------------------------------------------------------------- */

        public async Task<List<Transaction>> GetTransactions(string brokerId = null)
        {
            return await _hubConnection.InvokeAsync<List<Transaction>>(nameof(ITradeHub.GetTransactions), brokerId);
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

        public Task CreatedOrderBook(string username, OrderBook orderBook)
        {
            OnCreateOrderBook?.Invoke(username, orderBook);

            return Task.CompletedTask;
        }

        public Task DeletedOrderBook(string username, OrderBook orderBook)
        {
            OnDeleteOrderBook?.Invoke(username, orderBook);

            return Task.CompletedTask;
        }



        /* --------------------------------------------------------------- */
    }
}
