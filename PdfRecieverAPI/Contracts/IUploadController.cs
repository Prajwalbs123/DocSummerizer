using Microsoft.AspNetCore.Mvc;

namespace PdfRecieverAPI.Contracts
{
    public interface IUploadController
    {
        Task<IActionResult> IsFeatureEnabled();
        Task<IActionResult> GetSummary(IFormFile file);
        Task<IActionResult> Upload(IFormFile file);
    }
}