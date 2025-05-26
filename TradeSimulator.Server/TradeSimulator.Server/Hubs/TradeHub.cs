using Microsoft.AspNetCore.SignalR;

namespace TradeSimulator.Server.Hubs
{
    public class TradeHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
