using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using PdfRecieverAPI.Services;
using GptDLL;
using QuerySearchDLL;
using StoreEmbeddingsDLL;
using PdfReadChunkDLL;
using PdfRecieverAPI.Contracts;

var builder = WebApplication.CreateBuilder(args);

//azure appconfiguration access for feature flag
var connectionString = builder.Configuration.GetConnectionString("DocConfig");
builder.Configuration.AddAzureAppConfiguration(options =>
{
	options.Connect(connectionString);
	options.UseFeatureFlags(featureFlagOptions =>
	{
		featureFlagOptions.SetRefreshInterval(TimeSpan.FromSeconds(5));
	});
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//Adding all the services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});
builder.Services.AddSingleton<IPdfReadChunk,PdfReadChunk>();
builder.Services.AddSingleton<IGptCall,GptCall>();
builder.Services.AddSingleton<IStoreEmbeddedData,StoreEmbeddedData>();
builder.Services.AddSingleton<IQueryService,QueryService>();
builder.Services.AddSingleton<IUploadService,UploadService>();
builder.Services.AddSingleton<IQuerySearch,QuerySearch>();
builder.Services.AddFeatureManagement();

//Adding Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("cors");
app.MapControllers();

app.Run();
