using fnf.DocQuery.API.Models;

namespace fnf.DocQuery.API.Contracts
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