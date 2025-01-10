namespace PdfRecieverAPI.Contracts
{
    public interface IUploadService
    {
        Task<string> GetSummaryAsync(IFormFile file);
        Task<string> Upload(IFormFile file);
        Task<string> DeleteAllFilesAsync();
    }
}