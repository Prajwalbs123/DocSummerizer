using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Search.Documents.Indexes.Models;
using fnf.DocQuery.AzureSearch.Contracts;
using Microsoft.Extensions.Configuration;

namespace fnf.DocQuery.AzureSearch.Services
{
    public class AddScoringProfile(IConfiguration configuration) : IAddScoringProfile
    {
        private readonly IConfiguration configuration = configuration;
        public ScoringProfile Add()
        {
            IDictionary<string, double> Weights = new Dictionary<string, double>();
            Weights.Add(new KeyValuePair<string, double>(configuration["ScoreSettings:FieldValue"]!, Convert.ToDouble(configuration["ScoreSettings:WeightValue"])));
            var scoringProfile = new ScoringProfile("embeddingprofile")
            {
                TextWeights = new TextWeights(Weights)
            };

            return scoringProfile;
        }

    }
}
