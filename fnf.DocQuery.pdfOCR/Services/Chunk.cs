using fnf.DocQuery.pdfOCR.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Tokenizers;

namespace fnf.DocQuery.pdfOCR.Services
{

    public class Chunk(IConfiguration configuration, ILogger<Chunk> _logger, IReadPdf readPdf) : IChunk
    {
        private readonly IConfiguration configuration = configuration;
        private readonly ILogger<Chunk> _logger = _logger;
        private readonly Tokenizer tokenizer = TiktokenTokenizer.CreateForModel(configuration["AzureCred:model"]!);
        private readonly int inputTokensize = Convert.ToInt32(Convert.ToInt32(configuration["AzureCred:modelMaxTokens"]) * 0.25);
        private readonly IReadPdf readPdf = readPdf;
        /// <summary>
        ///     chunks large text string using tokenizer class for embedding reasons.
        /// </summary>
        /// <param name="file">IFormFile pdffile</param>
        /// <param name="fileName">string fileName</param>
        /// <returns>list of string where each string represents a chunk</returns>
        public List<string> GetChunk(IFormFile file)
        {
            _logger.LogInformation("Chunking begins");
            List<string> chunks = [];
            try
            {
                string text = readPdf.GetText(file);
                int startIndex = 0;
                while (startIndex < text.Length)
                {
                    int EndIndex = startIndex + tokenizer.GetIndexByTokenCount(text, maxTokenCount: inputTokensize, out string? normalizedText, out int count);
                    EndIndex = Math.Min(EndIndex, text.Length - 1);
                    chunks.Add(text.Substring(startIndex, EndIndex - startIndex + 1)!);
                    startIndex += EndIndex - 1;
                }
                _logger.LogInformation("Chunking completed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return chunks;
        }
    }
}
