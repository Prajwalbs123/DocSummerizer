using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using fnf.DocQuery.OpenAi.Contracts;

namespace fnf.DocQuery.OpenAi.Services
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
        public async Task<string> GptResponse(string SystemMessage,String UserMessage)
        {
            _logger.LogInformation("Query and Context are being processed by LLM");
            string gptResponse = string.Empty;
            try
            {
                ChatHistory history = [];
                history.AddSystemMessage(SystemMessage);
                history.AddUserMessage(UserMessage);

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
    }
}
