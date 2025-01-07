
namespace GptDLL
{
	/// <summary>
	///		Interface for GptCall
	/// </summary>
	public interface IGptCall
	{
		Task<string> GptResponse(string query, string context, int noSentence, string reference);
		Task<string> GptSummarize(string? fileText, string? fileName);
	}
}