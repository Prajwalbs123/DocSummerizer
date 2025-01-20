using Azure.Search.Documents.Indexes.Models;
using fnf.DocQuery.AzureSearch.Contracts;
using Microsoft.Extensions.Configuration;

namespace fnf.DocQuery.AzureSearch.Services
{
    public class AddVectorSearch(IConfiguration configuration) : IAddVectorSearch
    {
        private readonly IConfiguration configuration = configuration;

        public VectorSearch Add()
        {
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
            return vectorProfile;
        }
    }
}
