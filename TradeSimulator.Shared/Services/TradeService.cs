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
    public class TradeService : IAsyncDisposable
    {
        private HubConnection _hubConnection;



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
