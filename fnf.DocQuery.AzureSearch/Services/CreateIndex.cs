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

                IDictionary<string, double> Weights = new Dictionary<string, double>();
                Weights.Add(new KeyValuePair<string, double>(configuration["ScoreSettings:FieldValue"]!, Convert.ToDouble(configuration["ScoreSettings:WeightValue"])));
                var scoringProfile = new ScoringProfile("embeddingprofile")
                {
                    TextWeights = new TextWeights(Weights)
                };

                var vectorProfile = new VectorSearch()
                {
                    Algorithms =
                    {
                        new HnswAlgorithmConfiguration("hnsw")
                        {
                            Parameters = new HnswParameters()
                            {
                                M = Convert.ToInt32(configuration["IndexSettings:biDirectionalLink(M)"]),
                                EfConstruction = Convert.ToInt32(configuration["IndexSettings:EfConstruct"]),
                                EfSearch = Convert.ToInt32(configuration["IndexSettings:EfSearch"])
                            }
                        }
                    },
                    Profiles =
                    {
                        new VectorSearchProfile("my-vector-profile", "hnsw")
                        {
                            VectorizerName = "my-vectorizer"
                        }
                    },
                    Vectorizers =
                    {
                        new AzureOpenAIVectorizer("my-vectorizer")
                        {
                            Parameters = new AzureOpenAIVectorizerParameters()
                            {
                                ApiKey = configuration["AzureCred:key"],
                                DeploymentName = configuration["AzureCred:embeddingModel"],
                                ModelName = configuration["AzureCred:embeddingModel"],
                                ResourceUri = new Uri(configuration["AzureCred:endpoint"]!)
                            }
                        }
                    }
                    
                };

                var index = new SearchIndex(configuration["SearchCred:index"])
                {
                    Fields = fields,
                    VectorSearch = vectorProfile
                };
                index.ScoringProfiles.Add(scoringProfile);

                await indexClient.CreateOrUpdateIndexAsync(index);
                logger.LogInformation("Index Created");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex.Message}");
            }
        }

    }
}
