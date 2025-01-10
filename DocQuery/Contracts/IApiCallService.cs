namespace DocQuery.Contracts
{
    public interface IApiCallService
    {
        Task<HttpResponseMessage> GetFileListAsync();
        Task<string> GetSummaryAsync(MultipartFormDataContent content);
        Task<string> PostFileAsync(MultipartFormDataContent content);
        Task<HttpResponseMessage> PostQueryAsync(HttpContent content);
        Task<string> DeleteAllFiles();
    }
}