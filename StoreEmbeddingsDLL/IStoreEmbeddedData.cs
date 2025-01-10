
namespace StoreEmbeddingsDLL
{
	public interface IStoreEmbeddedData
	{
		Task Embed(List<string> chunks, string fileName);
		Task<string> DeleteAllFiles();
	}
}