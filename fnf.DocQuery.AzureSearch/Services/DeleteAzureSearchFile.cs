using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using System.Text.Json.Nodes;
using System.Text.Json;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using fnf.DocQuery.AzureSearch.Contracts;

namespace fnf.DocQuery.AzureSearch.Services
{
    public class DeleteAzureSearchFile(IConfiguration configuration, ILogger<DeleteAzureSearchFile> logger) : IDeleteAzureSearchFile
    {
        private readonly ILogger<DeleteAzureSearchFile> _logger = logger;
        private readonly IConfiguration configuration = configuration;

        private readonly SearchClient searchClient = new SearchClient(
                new Uri(configuration["SearchCred:uri"]!),
                configuration["SearchCred:index"]!,
                new AzureKeyCredential(configuration["SearchCred:key"]!)
                );

        /// <summary>
        /// Delete file data from index based on file ID
        /// </summary>
        /// <returns>String: file Id</returns>
        public async Task<string> DeleteFile(string? fileId)
        {
            JsonNode res = string.Empty;
            try
            {
                _logger.LogInformation("Deleting data...");
                string? filterQuery;
                if (fileId != null)
                {
                    filterQuery = $"fileId eq '{fileId}'";
                }
                else
                {
                    filterQuery = null;
                }

                var searchAll = new SearchOptions()
                {
                    Filter = filterQuery
                };

                var AllFiles = await searchClient.SearchAsync<SearchDocument>(searchAll);
                var AllData = AllFiles.GetRawResponse().Content.ToString();
                JsonObject json = JsonSerializer.Deserialize<JsonObject>(AllData)!;
                var files = json["value"]?.AsArray()!;

                if (fileId != null) res = $"{json?["value"]?[0]?["reference"]!} is";
                else if (files.Count != 0) res = "All Files are";
                else res = "No file exists to be";

                foreach (var file in files)
                {
                    var deleteAction = IndexDocumentsAction.Delete(new SearchDocument { ["id"] = file?["id"]?.ToString() });
                    var batch = new IndexDocumentsBatch<SearchDocument>();
                    batch.Actions.Add(deleteAction);
                    await searchClient.IndexDocumentsAsync(batch);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return res.ToString();
        }

    }
}
