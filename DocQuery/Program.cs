using DocQuery;
using DocQuery.Contracts;
using DocQuery.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddTransient(sp => new HttpClient {
    BaseAddress = new Uri(configuration.GetSection("ApiEndpoint").Value!)
});
builder.Services.AddLogging();
builder.Services.AddScoped<IFeatureDataService,FeatureDataService>();
builder.Services.AddScoped<IStartUpService, StartUpService>();
builder.Services.AddScoped<IApiCallService,ApiCallService>();


var host = builder.Build();
var startService = host.Services.GetRequiredService<IStartUpService>();
await startService.Initialize();
var dataService = host.Services.GetRequiredService<IFeatureDataService>();
await dataService.GetFeatureStatusAsync();

await host.RunAsync();
