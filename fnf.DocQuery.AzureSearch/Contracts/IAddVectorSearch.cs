using Azure.Search.Documents.Indexes.Models;

namespace fnf.DocQuery.AzureSearch.Contracts
{
    public interface IAddVectorSearch
    {
        VectorSearch Add();
    }
}