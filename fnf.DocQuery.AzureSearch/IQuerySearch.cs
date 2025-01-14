
namespace QuerySearchDLL
{
	public interface IQuerySearch
	{
		Task<string> GetContext(IList<ReadOnlyMemory<float>> QueryEmpbeddings, string query, string? fileName);
		Task<Dictionary<string,string>> GetFilesFromIndex();
	}
}