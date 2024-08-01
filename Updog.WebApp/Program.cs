using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Updog.WebApp;
using MudBlazor.Services;
using Updog.WebApp.Services;
using Updog.Core;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// builder.Services.Configure<AppConfig>(builder.Configuration.Bind);

var apiKey = "REDACTED";
builder.Services.AddSingleton<UpBankApiClient>(_ => new(apiKey));

builder.Services.AddSingleton<ClipboardService>();
builder.Services.AddSingleton<LocalStorageService>();

await builder.Build().RunAsync();
