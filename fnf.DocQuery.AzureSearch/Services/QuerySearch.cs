using System.Text.Json;
using System.Text.Json.Nodes;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using fnf.DocQuery.AzureSearch.Contracts;
using fnf.DocQuery.OpenAi.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace fnf.DocQuery.AzureSearch.Services
{


    /// <summary>
    ///		Azure Ai search client Class to retreive relevent Index data 
    /// </summary>
    /// <param name="configuration">DI of IConfiguration - for getting SearchCredentials from appsettings.json</param>
    /// <param name="_logger">DI of Ilogger - console logger for Logging information and errors</param>
    public class QuerySearch(IConfiguration configuration, ILogger<QuerySearch> _logger, IGetEmbeddings getEmbeddings) : IQuerySearch
    {
        private readonly IConfiguration configuration = configuration;
        private readonly IGetEmbeddings getEmbeddings = getEmbeddings;
        private readonly ILogger<QuerySearch> _logger = _logger;

        private readonly SearchClient searchClient = new SearchClient(
                new Uri(configuration["SearchCred:uri"]!),
                configuration["SearchCred:index"]!,
                new AzureKeyCredential(configuration["SearchCred:key"]!)
                );


        /// <summary>
        ///     searching the specified index for the context based on query and fileName.
        /// </summary>
        /// <param name="query">string: user query</param>
        /// <param name="fileName">string nullable: filename for reference</param>
        /// <returns>string: Context for LLM</returns>
        public async Task<string> GetContext(string queryText, string? fileId)
        {

            _logger.LogInformation("Query Recieved by GetContext function of QueryClass");
            string SearchResponse = string.Empty;
            try
            {
                var queryEmbeddings = await getEmbeddings.QueryEmbeddings(queryText);
                string? filterQuery;
                if (fileId != null)
                {
                    filterQuery = $"fileId eq '{fileId}'";
                }
                else
                {
                    filterQuery = null;
                }


                SearchOptions searchOptions = new SearchOptions()
                {
                    QueryType = SearchQueryType.Full,
                    VectorSearch = new()
                    {
                        Queries = { new VectorizedQuery(queryEmbeddings[0]) { KNearestNeighborsCount = 4, Fields = { "embedding" } } }
                    },
                    Filter = filterQuery,
                    Size = 1
                };
                var result = await searchClient.SearchAsync<SearchDocument>(queryText, options: searchOptions);
                SearchResponse = result.GetRawResponse().Content.ToString();
                _logger.LogInformation("Processed query to get SearchResponse");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return SearchResponse;
        }

        /// <summary>
        ///     Get all the reference fileName stored in azure search index
        /// </summary>
        /// <returns>List of string: FileNames stored in azure search index</returns>
        public async Task<Dictionary<string, string>> GetFilesFromIndex()
        {
            _logger.LogInformation("Processing file Information...");
            Dictionary<string, string> UniqueFiles = new Dictionary<string, string>();

            try
            {
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
                    if (!UniqueFiles.ContainsKey(file?["fileId"]?.ToString()!))
                    {
                        UniqueFiles.Add(file?["fileId"]?.ToString()!, file?["reference"]?.ToString()!);
                    }
                }
                _logger.LogInformation($"{UniqueFiles.Count} files are present in index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return UniqueFiles;
        }
    }
}
