using System.Text.Json;
using PdfRecieverAPI.Models;

namespace PdfRecieverAPI.Contracts
{
    /// <summary>
    ///     Interface implemented by QueryService
    /// </summary>
    public interface IQueryService
    {
        Task<Dictionary<string,string>> GetFileList();
        Task<string> Query(QueryModel request);
    }
}