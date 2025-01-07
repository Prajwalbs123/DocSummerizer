using System.Text.Json;

namespace PdfRecieverAPI.Contracts
{
    /// <summary>
    ///     Interface implemented by QueryService
    /// </summary>
    public interface IQueryService
    {
        Task<List<string>> GetFileList();
        Task<string> Query(JsonElement request);
    }
}