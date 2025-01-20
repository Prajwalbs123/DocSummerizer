using Azure.Search.Documents.Indexes.Models;

namespace fnf.DocQuery.AzureSearch.Contracts
{
    public interface IAddScoringProfile
    {
        ScoringProfile Add();
    }
}