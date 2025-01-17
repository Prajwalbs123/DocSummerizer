namespace fnf.DocQuery.OpenAi.Contracts
{
    public interface IGetEmbeddings
    {
        Task<IList<ReadOnlyMemory<float>>> QueryEmbeddings(string query);
        Task<IList<ReadOnlyMemory<float>>> FileEmbeddings(List<string> chunks);
    }
}