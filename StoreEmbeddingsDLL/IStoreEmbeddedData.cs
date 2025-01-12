
namespace StoreEmbeddingsDLL
{
	public interface IStoreEmbeddedData
	{
		Task<IList<ReadOnlyMemory<float>>> QueryEmbed(string query);
        Task Embed(List<string> chunks, string fileName);
		Task<string> DeleteFile(string? fileId);
	}
}