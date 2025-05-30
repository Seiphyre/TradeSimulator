using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Utils;

namespace TradeSimulator.Shared.Services
{
    public delegate void OnCreateOrderBook(OrderBook orderBook);
    public delegate void OnDeleteOrderBook(OrderBook orderBook);

    public class TradeService : IAsyncDisposable
    {
        private HubConnection _hubConnection;

        public event OnCreateOrderBook OnCreateOrderBook;
        public event OnDeleteOrderBook OnDeleteOrderBook;


        /* --------------------------------------------------------------- */

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

            _hubConnection.On<OrderBook>("OnCreateOrderBook", (orderBook) =>
            {
                OnCreateOrderBook?.Invoke(orderBook);
            });

            _hubConnection.On<OrderBook>("OnDeleteOrderBook", (orderBook) =>
            {
                OnDeleteOrderBook?.Invoke(orderBook);
            });

            await _hubConnection.StartAsync();
        }

        public async Task Disconnect()
        {
            await _hubConnection.StopAsync();
        }

        public async Task<List<Ticker>> GetTickers()
        {
            return await _hubConnection.InvokeAsync<List<Ticker>>("GetAllTickers");
        }

        public async Task<Ticker> GetTickerById(string tickerId)
        {
            return await _hubConnection.InvokeAsync<Ticker>("GetTickerById", tickerId);
        }

        public async Task<Broker> GetOrCreateBroker(string brokerId)
        {
            return await _hubConnection.InvokeAsync<Broker>("GetOrCreateBroker", brokerId);
        }

        public async Task<List<OrderBook>> GetOrderBooks(string brokerId = null)
        {
            return await _hubConnection.InvokeAsync<List<OrderBook>>("GetOrderBooks", brokerId);
        }

        public async Task<OrderBook> CreateOrderBook(string brokerId, string tickerId)
        {
            return await _hubConnection.InvokeAsync<OrderBook>("CreateOrderBook", brokerId, tickerId);
        }

        public async Task DeleteOrderBook(string orderBookId)
        {
            await _hubConnection.InvokeAsync("DeleteOrderBook", orderBookId);
        }

        /* --------------------------------------------------------------- */

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
