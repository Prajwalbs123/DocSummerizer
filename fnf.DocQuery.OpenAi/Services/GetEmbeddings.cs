using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Azure;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using System.Text.Json;
using fnf.DocQuery.OpenAi.Contracts;

namespace fnf.DocQuery.OpenAi.Services;


/// <summary>
///		Embedding the string list - chunks provided by PdfReadChunkDLL;
/// </summary>
/// <param name="configuration">IConfiguration - configuration for embeddings, search client</param>
/// <param name="_logger">ILogger - logger to log errors and information</param>
public class GetEmbeddings(IConfiguration configuration, ILogger<GetEmbeddings> _logger) : IGetEmbeddings
{
    private readonly IConfiguration configuration = configuration;
    private readonly ILogger<GetEmbeddings> _logger = _logger;

#pragma warning disable SKEXP0010
    private readonly AzureOpenAITextEmbeddingGenerationService textEmbeddingService = new(
            deploymentName: configuration["AzureCred:embeddingModel"]!,
            endpoint: configuration["AzureCred:endpoint"]!,
            apiKey: configuration["AzureCred:key"]!
          );

    /// <summary>
    ///		Using Embedding model to convert user query to embeddings
    /// </summary>
    /// <param name="query">string: User Query</param>
    /// <returns>IList ReadOnly Float: Embeddings</returns>
    public async Task<IList<ReadOnlyMemory<float>>> QueryEmbeddings(string query)
    {
        IList<ReadOnlyMemory<float>> queryEmbeddings = [];
        try
        {
            queryEmbeddings = await textEmbeddingService.GenerateEmbeddingsAsync([query]);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
        return queryEmbeddings;
    }
    /// <summary>
    ///     carry out embedding of chunks and storing of those embeddings into azure search's indexes
    /// </summary>
    /// <param name="chunks">List of string:utilized as chunks to get back it's embedded data</param>
    /// <param name="fileName">string: filename for reference</param>
    /// <returns></returns>
    public async Task<IList<ReadOnlyMemory<float>>> FileEmbeddings(List<string> chunks)
    {
        IList<ReadOnlyMemory<float>> fileEmbeddings = [];
        try
        {
            fileEmbeddings = await textEmbeddingService.GenerateEmbeddingsAsync(chunks);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
        return fileEmbeddings;
    }
}
