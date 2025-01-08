using System.Text.Json.Nodes;
using System.Text.Json;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;

var path = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(path!,"appsettings.json"))
    .Build();

var section = configuration.GetSection("SearchCred");
var index = section.GetSection("index").Value;
var searchClient = new SearchClient(
    new Uri(section.GetSection("uri").Value!),
    index,
    new Azure.AzureKeyCredential(section.GetSection("key").Value!)
    );

var searchAll = new SearchOptions()
{
    Size = 1000
};

var AllFiles = await searchClient.SearchAsync<SearchDocument>("*", searchAll);
var AllData = AllFiles.GetRawResponse().Content.ToString();
JsonObject json = JsonSerializer.Deserialize<JsonObject>(AllData)!;
var files = json["value"]?.AsArray()!;

foreach (var file in files)
{
    var deleteAction = IndexDocumentsAction.Delete(new SearchDocument { ["id"] = file?["id"]?.ToString() });
    var batch = new IndexDocumentsBatch<SearchDocument>();
    batch.Actions.Add(deleteAction);
    await searchClient.IndexDocumentsAsync(batch);
}

Console.WriteLine(index+" Data Deleted");