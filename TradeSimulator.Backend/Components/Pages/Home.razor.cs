using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TradeSimulator.Backend.Models;

namespace TradeSimulator.Backend.Components.Pages
{
    public class HomeBase : ComponentBase
    {
        private readonly string _defaultHubURL = "https://localhost:7125/trade-hub";

        [Inject] NavigationManager Navigation { get; set; }

        protected HubConnection hubConnection;
        protected List<string> Messages = [];

        protected string UserName = "Server";

        protected override async Task OnInitializedAsync()
        {
            await Connect();
        }

        protected async Task Connect()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_defaultHubURL, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(GenerateJwtToken());
                })
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<string, string>("OnConnected", (user, message) =>
            {
                if (user == UserName)
                    return;

                var encodedMsg = $"{user}: {message}";
                Messages.Add(encodedMsg);

                InvokeAsync(StateHasChanged);
            });

            hubConnection.On<string, string>("OnDisconnected", (user, message) =>
            {
                if (user == UserName)
                    return;

                var encodedMsg = $"{user}: {message}";
                Messages.Add(encodedMsg);

                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        private string GenerateJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("70686367d1ce87264121d74a5abec5eeae3f54c70f0204017b9ab758e69a7e8d"));

            var token = new JwtSecurityToken(
                claims: new List<Claim>() { new Claim(ClaimTypes.Name, UserName) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
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
