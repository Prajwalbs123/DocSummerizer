using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GptDLL
{

	public class GptCall(IConfiguration configuration, ILogger<GptCall> _logger) : IGptCall
	{
		//private static readonly ChatHistory history = [];
		private readonly IConfiguration configuration = configuration;
		private readonly ILogger<GptCall> _logger = _logger;
		private readonly AzureOpenAIChatCompletionService chatService = new AzureOpenAIChatCompletionService(
				configuration["AzureCred:model"]!,
				configuration["AzureCred:endpoint"]!,
				configuration["AzureCred:key"]!
				);

		/// <summary>
		/// Asyncronous call to get LLM response for the provided query and context
		/// </summary>
		/// <param name="query">User query</param>
		/// <param name="context">Context extracted from azure search</param>
		/// <param name="noSentence">Integer to limit the llm output size</param>
		/// <returns>string - llm response</returns>
		public async Task<string> GptResponse(string query, string context, int noSentence, string reference)
		{
			_logger.LogInformation("Query and Context are being processed by LLM");
			string gptResponse = string.Empty;
			try
			{
				ChatHistory history = [];
				history.AddSystemMessage($"You are a useful assistant who provides appropriate responses to user queries based on the provided context.");
				history.AddUserMessage($@"-If there is appropriate context : {context} available for the user query: {query}, summarize the context based on the query in at most the specified number of sentences : {noSentence}, considering the given reference : {reference}.
				 -Else, generate the response based on references from the Internet for the query : {query} in at most the specified number of sentences : {noSentence}, and specify the unavailability of context clearly to the user, and provide references for your response.
				output format:
					Ai response: [Your response here, along with Context Availability Information]

					Ai References: [Reference links to Internet-based response]

					Context Reference: {reference}");

				var response = await chatService.GetChatMessageContentAsync(history);
				_logger.LogInformation("successful response from LLM model "); 
				gptResponse = response.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex.Message}");
			}

			return gptResponse;

		}

		/// <summary>
		/// asyncronous call to LLm to summarize the text provided
		/// </summary>
		/// <param name="fileText">string text extracted from PDF, will be used by llm as context</param>
		/// <param name="fileName">reference filename from where the text has be extracted</param>
		/// <returns>string - llm response<</returns>
		public async Task<string> GptSummarize(string? fileText, string? fileName)
		{
			_logger.LogInformation("fileText is being summarized by LLM");
			string GptSummaryResponse = string.Empty;
			try
            {
                ChatHistory history = [];
                history.AddSystemMessage("You are a useful assistant who summarizes the provided fileText");
				history.AddUserMessage($@"Summarize {fileText}, referenced from {fileName}");
				var response = await chatService.GetChatMessageContentAsync(history);
				_logger.LogInformation("Summarization completed");
				GptSummaryResponse = response.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex.Message}");
			}

			return GptSummaryResponse;
		}
	}
}
