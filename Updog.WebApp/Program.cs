using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Updog.WebApp;
using Updog.WebApp.Services;

System.Globalization.CultureInfo.CurrentCulture = new("en-AU");
System.Globalization.CultureInfo.CurrentUICulture = new("en-AU");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddSingleton<ClipboardService>();
builder.Services.AddSingleton<LocalStorageService>();
builder.Services.AddSingleton<SessionStorageService>();
builder.Services.AddSingleton<StateManager>();

await builder.Build().RunAsync();
