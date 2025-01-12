using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Azure;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace StoreEmbeddingsDLL {


	/// <summary>
	///		Embedding the string list - chunks provided by PdfReadChunkDLL;
	/// </summary>
	/// <param name="configuration">IConfiguration - configuration for embeddings, search client</param>
	/// <param name="_logger">ILogger - logger to log errors and information</param>
	public class StoreEmbeddedData(IConfiguration configuration, ILogger<StoreEmbeddedData> _logger) : IStoreEmbeddedData
	{
		private readonly IConfiguration configuration = configuration;
		private readonly ILogger<StoreEmbeddedData> _logger = _logger;
		
		#pragma warning disable SKEXP0010
		private readonly AzureOpenAITextEmbeddingGenerationService textEmbeddingService = new(
				deploymentName: configuration["AzureCred:embeddingModel"]!,
				endpoint: configuration["AzureCred:endpoint"]!,
				apiKey: configuration["AzureCred:key"]!
			  );

		private readonly SearchClient searchClient = new SearchClient(
				new Uri(configuration["SearchCred:uri"]!),
				configuration["SearchCred:index"]!,
				new AzureKeyCredential(configuration["SearchCred:key"]!)
				);

		/// <summary>
		///		Using Embedding model to convert user query to embeddings
		/// </summary>
		/// <param name="query">string: User Query</param>
		/// <returns>IList ReadOnly Float: Embeddings</returns>
		public async Task<IList<ReadOnlyMemory<float>>> QueryEmbed(string query)
		{
			IList<ReadOnlyMemory<float>> QueryEmbeddings = [];
			try
			{
				QueryEmbeddings = await textEmbeddingService.GenerateEmbeddingsAsync([query]);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex.Message}");
			}
			return QueryEmbeddings;

		}
		/// <summary>
		///     carry out embedding of chunks and storing of those embeddings into azure search's indexes
		/// </summary>
		/// <param name="chunks">List of string:utilized as chunks to get back it's embedded data</param>
		/// <param name="fileName">string: filename for reference</param>
		/// <returns></returns>
		public async Task Embed(List<string> chunks, string fileName)
		{
			try
			{
				var fileList = Path.GetFileNameWithoutExtension(fileName);
				IList<ReadOnlyMemory<float>> ret = await textEmbeddingService.GenerateEmbeddingsAsync(chunks);

				var documents = new List<SearchDocument>();
				
				Guid fileId = Guid.NewGuid();
				for (int i = 0; i < chunks.Count; i++)
				{
					var document = new SearchDocument
					{
						{"id",$"{i}_{fileList}_{Guid.NewGuid()}" },
						{"embedding",ret[i].Span.ToArray() },
						{"original_text", new List<string> {chunks[i]} },
						{"reference", fileName },
						{"fileId",fileId.ToString() }
					};

					documents.Add(document);
				}

				Response<IndexDocumentsResult> res = await searchClient.UploadDocumentsAsync(documents);
				_logger.LogInformation($"Embedded Documents Uploaded, {res}");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex.Message}");
			}
		}

        /// <summary>
        /// Delete all Index data
        /// </summary>
        /// <returns>String: index name</returns>
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
				if (fileId != null) res = json?["value"]?[0]?["reference"]!;
				else res = "All Files Deleted";

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
