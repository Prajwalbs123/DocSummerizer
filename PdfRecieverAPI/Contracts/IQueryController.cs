using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PdfRecieverAPI.Contracts
{
    public interface IQueryController
    {
        Task<IActionResult> GetFileList();
        Task<IActionResult> Query([FromBody] JsonElement request);
    }
}