using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TradeSimulator.Frontend.Blazor.Components.Components;
using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.Blazor.Components.Pages
{
    public class OrderBookPageBase : ComponentBase
    {
        [Parameter]
        public string TickerId { get; set; }

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] TradeService TradeService { get; set; }


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
    }
}
