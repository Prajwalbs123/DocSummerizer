using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using fnf.DocQuery.AzureSearch.Model;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Logging;
using fnf.DocQuery.AzureSearch.Contracts;

namespace fnf.DocQuery.AzureSearch.Services
{
    public class CreateIndex(IConfiguration configuration,ILogger<CreateIndex> logger) : ICreateIndex
    {
        private readonly ILogger<CreateIndex> logger = logger;
        private readonly IConfiguration configuration = configuration;
        private readonly SearchIndexClient indexClient = new(
                new Uri(configuration["SearchCred:uri"]!),
                new AzureKeyCredential(configuration["SearchCred:key"]!)
                );

        public async Task Create()
        {
            try
            {
                logger.LogInformation("Creating Index....");
                var fieldBuilder = new FieldBuilder();
                var fields = fieldBuilder.Build(typeof(IndexModel));
                var vectorProfile = new VectorSearch
                {


                    Algorithms =
                    {
                        new HnswAlgorithmConfiguration("hnsw")
                        {
                            Parameters = new HnswParameters
                            {
                                M = 4,
                                EfConstruction =200,
                                EfSearch = 100
                            }
                        }
                    },
                    Profiles =
                    {

                        new VectorSearchProfile("my-vector-profile","hnsw")
                    }
                };

                var index = new SearchIndex(configuration["SearchCred:index"])
                {
                    Fields = fields,
                    VectorSearch = vectorProfile
                };

                await indexClient.CreateOrUpdateIndexAsync(index);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex.Message}");
            }
        }

    }
}
