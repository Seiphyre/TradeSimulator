using Microsoft.AspNetCore.Components;
using MudBlazor;
using TradeSimulator.Shared.Models;

namespace TradeSimulator.Frontend.Blazor.Components.Components
{
    public class CreateOrderBookDialogBase : ComponentBase
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

        [Parameter] public List<Ticker> Tickers { get; set; }

        protected MudForm Form { get; set; }
        protected Ticker SelectedTicker { get; set; }



        /* ------------------------------------------------ */

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var options = MudDialog.Options with
            {
                FullWidth = true,
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,
                BackdropClick = false
            };

            MudDialog.SetOptionsAsync(options);
        }



        /* ------------------------------------------------ */

        protected async Task Submit()
        {
            await Form.Validate();

            if (Form.IsValid)
                MudDialog.Close(DialogResult.Ok(SelectedTicker));
        }

        protected void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
