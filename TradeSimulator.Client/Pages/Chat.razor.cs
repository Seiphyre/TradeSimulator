using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace TradeSimulator.BlazorClient.Pages
{
    public class ChatBase : ComponentBase, IAsyncDisposable
    {
        [Inject] protected NavigationManager Navigation { get; set; }

        protected HubConnection hubConnection;
        protected List<string> messages = [];
        protected string userInput;
        protected string messageInput;



        /* -------------------------------------------------------------- */

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);
                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }



        /* -------------------------------------------------------------- */

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }



        /* -------------------------------------------------------------- */

        protected async Task Send()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", userInput, messageInput);
            }
        }

        public bool IsConnected =>
            hubConnection?.State == HubConnectionState.Connected;

    }
}
