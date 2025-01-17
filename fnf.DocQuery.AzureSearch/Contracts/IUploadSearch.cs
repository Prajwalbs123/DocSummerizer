namespace fnf.DocQuery.AzureSearch.Contracts
{
    public interface IUploadSearch
    {
        Task UploadEmbeddings(List<string> chunks, string fileName);
    }
}