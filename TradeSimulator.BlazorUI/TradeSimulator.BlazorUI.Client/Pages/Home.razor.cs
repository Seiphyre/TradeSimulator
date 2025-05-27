using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace TradeSimulator.BlazorUI.Client.Pages
{
    public class HomeBase: ComponentBase, IAsyncDisposable
    {

        private readonly string _defaultHubURL = "https://localhost:7225/trade-hub";

        [Inject] NavigationManager Navigation { get; set; }

        protected string BrokerId;

        private HubConnection hubConnection;
        private List<string> messages = [];
        private string userInput;
        private string messageInput;

        protected override void OnInitialized()
        {
            BrokerId = GenerateRandomBrokerId();

            hubConnection = new HubConnectionBuilder()
                .WithUrl(_defaultHubURL)
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);

                InvokeAsync(StateHasChanged);
            });
        }

        protected async Task Connect()
        {
            Console.WriteLine("Connecting...");

            try
            {
                await hubConnection.StartAsync();


                Console.WriteLine("Connected !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect.");
            }
        }

        protected async Task Disconnect()
        {
            await hubConnection.StopAsync();
        }

        private async Task Send()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", userInput, messageInput);
            }
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        public bool IsDisconnected => hubConnection?.State == HubConnectionState.Disconnected;

        private string GenerateRandomBrokerId()
        {
            return "Victor";
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}
