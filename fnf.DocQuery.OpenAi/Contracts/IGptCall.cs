namespace fnf.DocQuery.OpenAi.Contracts
{
    /// <summary>
    ///		Interface for GptCall
    /// </summary>
    public interface IGptCall
    {
        Task<string> GptResponse(string SystemMessage,string UserMessage);
    }
}