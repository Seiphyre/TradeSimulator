using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace TradeSimulator.Server.Components.Pages
{
    public class HomeBase : ComponentBase
    {
        private readonly string _defaultHubURL = "https://localhost:7225/trade-hub";

        [Inject] NavigationManager Navigation { get; set; }

        protected HubConnection hubConnection;
        protected List<string> Messages = [];

        protected override async Task OnInitializedAsync()
        {
            await Connect();
        }

        protected async Task Connect()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_defaultHubURL)
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<string, string>("OnConnected", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Messages.Add(encodedMsg);

                InvokeAsync(StateHasChanged);
            });

            hubConnection.On<string, string>("OnDisconnected", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Messages.Add(encodedMsg);

                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        public bool IsDisconnected => hubConnection?.State == HubConnectionState.Disconnected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}
