using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TradeSimulator.Backend.Repositories;
using TradeSimulator.Shared.Models;

namespace TradeSimulator.Backend.Hubs
{
    public class TradeHub : Hub
    {
        private readonly TickerRepository _tickerRepository;

        public TradeHub(TickerRepository tickerRepository)
        {
            _tickerRepository = tickerRepository;
        }


        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("OnConnectedAsync");
            await Clients.All.SendAsync("OnConnected", UserName, "Connected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("OnDisconnected");
            await Clients.All.SendAsync("OnDisconnected", UserName, "Disconnected");
        }

        public IEnumerable<Ticker> GetAllTickers()
        {
            return _tickerRepository.GetAll();
        }

        public Ticker GetTickerById(string id)
        {
            return _tickerRepository.GetById(id);
        }

        private string UserName => Context.User?.Identity?.Name ?? Context.ConnectionId;
    }
}
