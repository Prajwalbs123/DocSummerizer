namespace fnf.DocQuery.AzureSearch.Contracts
{
    public interface IDeleteAzureSearchFile
    {
        Task<string> DeleteFile(string? fileId);
    }
}