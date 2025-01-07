
namespace QuerySearchDLL
{
	public interface IQuerySearch
	{
		Task<string> GetContext(string query, string? fileName);
		Task<List<string>> GetFilesFromIndex();
	}
}