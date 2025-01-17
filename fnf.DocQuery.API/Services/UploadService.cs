using fnf.DocQuery.API.Contracts;
using fnf.DocQuery.AzureSearch.Contracts;
using fnf.DocQuery.OpenAi.Contracts;
using fnf.DocQuery.pdfOCR.Contracts;

namespace fnf.DocQuery.API.Services
{
    public class UploadService(IConfiguration configuration,IChunk pdfchunk,IPdfSummary pdfSummary, ILogger<UploadService> _logger, IGptCall gptCall,IUploadSearch uploadSearch,IDeleteAzureSearchFile deleteSearchFile) : IUploadService
    {
        private readonly IConfiguration configuration = configuration;
        private readonly ILogger<UploadService> _logger = _logger;
        private readonly IChunk pdfchunk = pdfchunk;
        private readonly IPdfSummary pdfSummary = pdfSummary;
        private readonly IGptCall gptCall = gptCall;
        private readonly IUploadSearch uploadSearch = uploadSearch;
        private readonly IDeleteAzureSearchFile deleteSearchFile = deleteSearchFile;

        /// <summary>
        ///     processes PDF file and returns it's summary. 
        /// </summary>
        /// <param name="file">input pdf file: IFormfile</param>
        /// <returns>string: response</returns>
        public async Task<string> GetSummaryAsync(IFormFile file)
        {
            string response = string.Empty;
            try
            {
                if (file == null || file.Length == 0)
                    return "No file uploaded.";

                string? fileText;
                fileText = pdfSummary.GetSummary(file);
                string SystemMessage = File.ReadAllText(configuration["SummarySystemPromptPath"]!);
                string UserMessage = File.ReadAllText(configuration["SummaryUserPromptPath"]!)
                    .Replace("{fileText}", fileText)
                    .Replace("{FileName}", file.FileName);

                response = await gptCall.GptResponse(SystemMessage,UserMessage);
                _logger.LogInformation("Gpt summarization response processed");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return response;

        }

        /// <summary>
        ///     Chunks the input PDf file and upload it to azure ai search index.
        /// </summary>
        /// <param name="file">PDF file: IFormfile</param>
        /// <returns>File upload status: string</returns>
        public async Task<string> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return "No file to uploaded.";

                await uploadSearch.UploadEmbeddings(pdfchunk.GetChunk(file),file.FileName);
                _logger.LogInformation($"{file.FileName} Upload Successful");
                return $"{file.FileName} uploaded successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: {ex.Message}");
                return $"{file.FileName} uploaded failed";
            }
        }

        public async Task<string> DeleteFileAsync(string? fileId)
        {
            string FileName = string.Empty;
            try
            {
                FileName = await deleteSearchFile.DeleteFile(fileId);
                _logger.LogInformation($"{FileName} Data Deleted Successful");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return FileName;
        }

    }
}
