namespace fnf.DocQuery.API.Contracts
{
    public interface IUploadService
    {
        Task<string> GetSummaryAsync(IFormFile file);
        Task<string> Upload(IFormFile file);
        Task<string> DeleteFileAsync(string? fileId);
    }
}