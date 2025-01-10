using System.Drawing;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace QuerySearchDLL
{


	/// <summary>
	///		Azure Ai search client Class to retreive relevent Index data 
	/// </summary>
	/// <param name="configuration">DI of IConfiguration - for getting SearchCredentials from appsettings.json</param>
	/// <param name="_logger">DI of Ilogger - console logger for Logging information and errors</param>
	public class QuerySearch(IConfiguration configuration, ILogger<QuerySearch> _logger) : IQuerySearch
	{
		private readonly IConfiguration configuration = configuration;
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
		public async Task<string> GetContext(IList<ReadOnlyMemory<float>> QueryEmbbeddings,string queryText, string? fileName)
		{
			_logger.LogInformation("Query Recieved by GetContext function of QueryClass");
			string SearchResponse = string.Empty;

			try
			{
				string? filterQuery;
				if (fileName != null)
				{
					filterQuery = $"reference eq '{fileName}'";
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
						Queries = { new VectorizedQuery(QueryEmbbeddings[0]){ KNearestNeighborsCount=4,Fields = { "embedding" } } }
					},
					Filter = filterQuery,
					Size = 1
				};
				var result = await searchClient.SearchAsync<SearchDocument>(queryText,options: searchOptions);
				SearchResponse = result.GetRawResponse().Content.ToString();
				_logger.LogInformation("Processed query to get SearchResponse");
			}
			catch (Exception ex)
			{
				_logger.LogError($"error: {ex.Message}");
			}

			return SearchResponse;
		}

		/// <summary>
		///     Get all the reference fileName stored in azure search index
		/// </summary>
		/// <returns>List of string: FileNames stored in azure search index</returns>
		public async Task<List<string>> GetFilesFromIndex()
		{
			_logger.LogInformation("Processing file Information");
			List<string> resFiles = [];
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
				SortedSet<string> uniqueFiles = new SortedSet<string>();

				foreach (var file in files)
				{
					uniqueFiles.Add(file?["reference"]?.ToString()!);
				}
				foreach (var file in uniqueFiles)
				{
					resFiles.Add(file!);
				}
				_logger.LogInformation($"{resFiles.Count} files are present in index");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex.Message}");
			}

			return resFiles;
		}
	}
}
