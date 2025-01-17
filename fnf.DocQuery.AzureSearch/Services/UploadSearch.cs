using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using fnf.DocQuery.AzureSearch.Contracts;
using fnf.DocQuery.OpenAi.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace fnf.DocQuery.AzureSearch.Services
{
    public class UploadSearch(IConfiguration configuration, IGetEmbeddings getEmbeddings, ILogger<UploadSearch> logger) : IUploadSearch
    {
        private readonly IConfiguration configuration = configuration;
        private readonly ILogger<UploadSearch> logger = logger;
        private readonly IGetEmbeddings getEmbeddings = getEmbeddings;


        private readonly SearchClient searchClient = new SearchClient(
                new Uri(configuration["SearchCred:uri"]!),
                configuration["SearchCred:index"]!,
                new AzureKeyCredential(configuration["SearchCred:key"]!)
                );
        public async Task UploadEmbeddings(List<string> chunks, string fileName)
        {
            logger.LogInformation("Uploading documents...");
            try
            {
                var fileList = Path.GetFileNameWithoutExtension(fileName);
                var documents = new List<SearchDocument>();
                IList<ReadOnlyMemory<float>> fileEmbeddings = await getEmbeddings.FileEmbeddings(chunks);
                Guid fileId = Guid.NewGuid();
                for (int i = 0; i < chunks.Count; i++)
                {
                    var document = new SearchDocument
                    {
                        {"id",$"{i}_{fileList}_{Guid.NewGuid()}" },
                        {"embedding",fileEmbeddings[i].Span.ToArray() },
                        {"original_text", new List<string> {chunks[i]} },
                        {"reference", fileName },
                        {"fileId",fileId.ToString() }
                    };

                    documents.Add(document);
                }

                Response<IndexDocumentsResult> res = await searchClient.UploadDocumentsAsync(documents);
                logger.LogInformation($"Embedded Documents Uploaded, {res}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex.Message}");
            }
        }
    }
}
