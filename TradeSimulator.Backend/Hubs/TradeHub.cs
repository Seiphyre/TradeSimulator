using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TradeSimulator.Backend.Hubs
{
    public class TradeHub : Hub
    {
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

        private string UserName => Context.User?.Identity?.Name ?? Context.ConnectionId;
    }
}
