using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;
using TradeSimulator.Server.Client.Pages;
using TradeSimulator.Server.Components;
using TradeSimulator.Server.Hubs;

namespace TradeSimulator.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /* -- Add services to the container. --------------------------- */

            builder.Services.AddSignalR();

            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddMudServices();

            var app = builder.Build();



            /* -- Configure the HTTP request pipeline ---------------------- */

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.MapHub<TradeHub>("/trade-hub");

            app.Run();
        }
    }
}
