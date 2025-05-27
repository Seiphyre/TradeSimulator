using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class HomeBase : ComponentBase, IAsyncDisposable
    {

        private readonly string _defaultHubURL = "http://localhost:5038/trade-hub";

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        protected string BrokerId;

        private HubConnection hubConnection;
        private List<string> messages = [];
        private string userInput;
        private string messageInput;

        private bool _isConnecting = false;
        private bool _isDisconnecting = false;

        protected override void OnInitialized()
        {
            BrokerId = GenerateRandomBrokerId();

            hubConnection = new HubConnectionBuilder()
                .WithUrl(_defaultHubURL, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(GenerateJwtToken());
                })
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);

                InvokeAsync(StateHasChanged);
            });
        }

        private string GenerateJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("70686367d1ce87264121d74a5abec5eeae3f54c70f0204017b9ab758e69a7e8d"));

            var token = new JwtSecurityToken(
                claims: new List<Claim>() { new Claim(ClaimTypes.Name, BrokerId) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected async Task Connect()
        {
            Console.WriteLine("Connecting...");

            _isConnecting = true;

            try
            {
                await hubConnection.StartAsync();

                Console.WriteLine("Connected !");
                Snackbar.Add($"Successfully Connected to hub", Severity.Success);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to connect.");
                Snackbar.Add($"Failed to connect. {exception.Message}", Severity.Error);
            }
            finally
            {
                _isConnecting = false;
            }
        }

        protected async Task Disconnect()
        {
            Console.WriteLine("Disconnecting...");

            _isDisconnecting = true;

            try
            {
                await hubConnection.StopAsync();

                Console.WriteLine("Disconnected.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to disconnect.");
                Snackbar.Add($"Failed to disconnect. {exception.Message}", Severity.Error);
            }
            finally
            {
                _isDisconnecting = false;
            }
        }

        private async Task Send()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", userInput, messageInput);
            }
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;
        public bool IsConnecting => _isConnecting;
        public bool IsDisconnecting => _isDisconnecting;

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
