using Blazored.LocalStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using MudBlazor.Services;
using TommyChat.FrontEnd;
using TommyChat.FrontEnd.Auth;
using TommyChat.FrontEnd.Repositories;
using TommyChat.FrontEnd.Sevices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7067/") });
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopEnd;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 2000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 10;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
});
builder.Services.AddSweetAlert2();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationProviderJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<HubConnection>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();

    return new HubConnectionBuilder()
        .WithUrl("https://localhost:7067/Notifyhub", options =>
        {
            options.AccessTokenProvider = async () =>
            {
                var token = await localStorage.GetItemAsync<string>("JwtKey");
                return token;
            };
        })
        .WithAutomaticReconnect()
        .Build();
});

await builder.Build().RunAsync();
