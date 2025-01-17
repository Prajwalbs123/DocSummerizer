using System.Text.Json;
using System.Text.Json.Nodes;
using fnf.DocQuery.API.Contracts;
using fnf.DocQuery.API.Models;
using Microsoft.SemanticKernel;
using fnf.DocQuery.AzureSearch.Contracts;
using fnf.DocQuery.OpenAi.Contracts;

namespace fnf.DocQuery.API.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_logger">DI of ILogger: To Log Information and Errors</param>
    /// <param name="querySearch">DI of IQuerySearc: DLL to retrive context from Azure ai search index</param>
    /// <param name="gptCall">DI of IGptCall: RAG using context and user Query</param>
    public class QueryService(ILogger<QueryService> _logger, IQuerySearch querySearch, IGptCall gptCall,IConfiguration configuration) : IQueryService
    {
        private readonly ILogger<QueryService> _logger = _logger;
        private readonly IQuerySearch querySearch = querySearch;
        private readonly IGptCall gptCall = gptCall;
        private readonly IConfiguration configuration = configuration;
        /// <summary>
        ///		Get Documents names from search index
        /// </summary>
        /// <returns>String List: file names from azure ai search index</returns>
        public async Task<Dictionary<string, string>> GetFileList()
        {
            _logger.LogInformation("GetFileList request received at QueryService");
            Dictionary<string, string> files = [];
            try
            {
                files = await querySearch.GetFilesFromIndex();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return files;
        }

        /// <summary>
        ///		Processes query based on filename,number of sentences and gives LLM response
        /// </summary>
        /// <param name="request">JsonElement: Parsed user request</param>
        /// <returns>LLM response to user query based on user specified conditions</returns>
        public async Task<string> Query(QueryModel request)
        {
            _logger.LogInformation("User Request posted to Query service");
            string GptResponse = string.Empty;
            try
            {
                //extracting data from request for processing
                string? query = request.Message;
                int noSentence = request.NoSentence;
                string? fileId = request.FileId;

                //call to Azure search to get context;
                string rawResponse = await querySearch.GetContext(query!, fileId);

                //Deserializing rawResponse from azure search
                JsonObject jsonValue = JsonSerializer.Deserialize<JsonObject>(rawResponse)!;
                JsonArray? jsonArray = jsonValue?["value"]?.AsArray();

                //reassigning reference based on search Score and search result
                JsonNode? Message, searchScore, reference;
                if (jsonArray?.Count != 0)
                {
                    searchScore = jsonValue?["value"]?[0]?["@search.score"];
                    Message = jsonValue?["value"]?[0]?["original_text"]?[0];
                    reference = jsonValue?["value"]?[0]?["reference"];
                }
                else
                {
                    _logger.LogWarning("Context Unavailable");
                    searchScore = 0.0;
                    Message = "";
                    reference = "Internet";
                }
                _logger.LogInformation("Query Processed");

                string SystemMessage = File.ReadAllText(configuration["QuerySystemPromptPath"]!);
                string UserMessage = File.ReadAllText(configuration["QueryUserPromptPath"]!);
                UserMessage = UserMessage.Replace("{context}", Message?.ToString())
                    .Replace("{query}", query)
                    .Replace("{reference}", reference?.ToString())
                    .Replace("{noSentence}", noSentence.ToString());

                GptResponse = await gptCall.GptResponse(SystemMessage,UserMessage);
                GptResponse += "\n\nSearchScore: " + searchScore!.ToString();

                _logger.LogInformation("Response from LLM recieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return GptResponse;
        }
    }
}
