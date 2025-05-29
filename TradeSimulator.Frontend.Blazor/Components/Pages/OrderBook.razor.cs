using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class OrderBookBase : ComponentBase
    {

        private readonly string _defaultHubURL = "http://localhost:5038/trade-hub";

        [Parameter]
        public string TickerId { get; set; }

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] TradeService TradeService { get; set; }

        protected string BrokerId;
        protected List<Ticker> Tickers;

        private List<string> messages = [];
        private string userInput;
        private string messageInput;

        private bool _isConnecting = false;
        private bool _isDisconnecting = false;

        protected Ticker _ticker;
        protected bool _isLoading = false;


        /* ---------------------------------------------------------- */

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (!TradeService.IsConnected)
                Navigation.NavigateTo("/");

            _ticker = await TradeService.GetTickerById(TickerId);

            _isLoading = false;
        }

        protected async Task ConnectAndGetTickers()
        {
            await Connect();
            await GetTickers();
        }

        protected async Task Connect()
        {
            Console.WriteLine("Connecting...");

            _isConnecting = true;

            try
            {
                await TradeService.Connect(_defaultHubURL, BrokerId);

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
                await TradeService.Disconnect();

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

        protected async Task GetTickers()
        {
            Tickers = await TradeService.GetTickers();

            foreach (var ticker in Tickers)
                Console.WriteLine($"[{ticker.Id}] {ticker.DisplayName} ({ticker.Orders.Count})");
        }

        protected async Task OpenOrderBook()
        {
            var parameters = new DialogParameters<OpenOrderBookDialog>
            {
                { x => x.Tickers, Tickers },
            };

            var dialog = await DialogService.ShowAsync<OpenOrderBookDialog>(null, parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Ticker ticker = result.Data as Ticker;

                Console.WriteLine(ticker.DisplayName);
            }
        }

        public bool IsConnected => TradeService.IsConnected;
        public bool IsConnecting => _isConnecting;
        public bool IsDisconnecting => _isDisconnecting;

        private string GenerateRandomBrokerId()
        {
            return "Victor";
        }
    }
}
