using Microsoft.FeatureManagement;
using fnf.DocQuery.API.Helper;
using fnf.DocQuery.AzureSearch.Services;
using fnf.DocQuery.AzureSearch.Model;
using fnf.DocQuery.AzureSearch.Contracts;

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
builder.Services.AddLogging();
builder.Services.AddFeatureManagement();
ServiceRegisterar.Register(builder.Services);

var app = builder.Build();

var createIndex = app.Services.GetRequiredService<ICreateIndex>();
await createIndex.Create();

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
