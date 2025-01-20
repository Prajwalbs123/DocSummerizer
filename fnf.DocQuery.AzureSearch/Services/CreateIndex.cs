using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using fnf.DocQuery.AzureSearch.Model;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Logging;
using fnf.DocQuery.AzureSearch.Contracts;

namespace fnf.DocQuery.AzureSearch.Services
{
    public class CreateIndex(IConfiguration configuration,ILogger<CreateIndex> logger,IAddScoringProfile addScoringProfile,IAddVectorSearch addVectorSearch) : ICreateIndex
    {
        private readonly ILogger<CreateIndex> logger = logger;
        private readonly IConfiguration configuration = configuration;
        private readonly SearchIndexClient indexClient = new(
                new Uri(configuration["SearchCred:uri"]!),
                new AzureKeyCredential(configuration["SearchCred:key"]!)
                );
        private readonly IAddScoringProfile addScoringProfile = addScoringProfile;
        private readonly IAddVectorSearch addVectorSearch = addVectorSearch;

        public async Task Create()
        {
            try
            {
                logger.LogInformation("Creating/Updating Index....");
                var fieldBuilder = new FieldBuilder();
                var fields = fieldBuilder.Build(typeof(IndexModel));
                var scoringProfile = addScoringProfile.Add();
                var vectorSearch = addVectorSearch.Add();
                

                var index = new SearchIndex(configuration["SearchCred:index"])
                {
                    Fields = fields,
                    VectorSearch = vectorSearch
                };
                index.ScoringProfiles.Add(scoringProfile);

                await indexClient.CreateOrUpdateIndexAsync(index);
                logger.LogInformation("Index Created/Updated");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex.Message}");
            }
        }

    }
}
