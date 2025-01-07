using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Azure;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
		///     carry out embedding of chunks and storing of those embeddings into azure search's indexes
		/// </summary>
		/// <param name="chunks">List of string:utilized as chunks to get back it's embedded data</param>
		/// <param name="fileName">string: filename for reference</param>
		/// <returns></returns>
		public async Task Embed(List<string> chunks, string fileName)
		{
			try
			{
				var fileList = fileName.Split('.');
				IList<ReadOnlyMemory<float>> ret = await textEmbeddingService.GenerateEmbeddingsAsync(chunks);

				var documents = new List<SearchDocument>();
				for (int i = 0; i < chunks.Count; i++)
				{
					var document = new SearchDocument
				{
					{"id",fileList[0]+"_"+i.ToString() },
					{"embedding",ret[i].Span.ToArray() },
					{"original_text", new List<string> {chunks[i]} },
					{"reference", fileName }
				};

					documents.Add(document);
				}

				Response<IndexDocumentsResult> res = await searchClient.UploadDocumentsAsync(documents);
				_logger.LogInformation($"Embedded Documents Uploaded, {res}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error: {ex.Message}");
			}
		}
	}
}
