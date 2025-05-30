using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using System.Text;
using TradeSimulator.Backend.Components;
using TradeSimulator.Backend.Hubs;
using TradeSimulator.Backend.Repositories;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Query.ContainsKey("access_token"))
                        {
                            context.Token = context.Request.Query["access_token"];
                        }
                        else if (context.Request.Headers.TryGetValue("Authorization", out var value) && value.Count > 0)
                        {
                            context.Token = value[0].Substring("Bearer ".Length);
                        }

                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    LifetimeValidator = (before, expires, token, param) =>
                    {
                        return expires > DateTime.UtcNow;
                    },
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("70686367d1ce87264121d74a5abec5eeae3f54c70f0204017b9ab758e69a7e8d")),
                };
            });

            builder.Services.AddMudServices();

            builder.Services.AddSingleton<TradeService>();

            builder.Services.AddSingleton<BrokerRepository>();
            builder.Services.AddSingleton<TickerRepository>();
            builder.Services.AddSingleton<OrderBookRepository>();
            builder.Services.AddSingleton<TransactionRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.UseAuthentication();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapHub<TradeHub>("/trade-hub");

            app.Run();
        }
    }
}
