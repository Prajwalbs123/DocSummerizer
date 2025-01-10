
namespace QuerySearchDLL
{
	public interface IQuerySearch
	{
		Task<string> GetContext(IList<ReadOnlyMemory<float>> QueryEmpbeddings, string query, string? fileName);
		Task<List<string>> GetFilesFromIndex();
	}
}