namespace fnf.DocQuery.AzureSearch.Contracts
{
    public interface IQuerySearch
    {
        Task<string> GetContext(string query, string? fileName);
        Task<Dictionary<string, string>> GetFilesFromIndex();
    }
}