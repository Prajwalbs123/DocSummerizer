using System.Text.Json.Nodes;
using System.Text.Json;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

var index = "demo-index-2";
var searchClient = new SearchClient(new Uri("https://research-paper-search-service.search.windows.net"), index, new Azure.AzureKeyCredential("RRZc2vkyxnIs3wMmBk4p1QM3zQi9teyqHVQhsa55xcAzSeBzJW8x"));

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