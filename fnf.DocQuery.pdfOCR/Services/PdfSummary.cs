using fnf.DocQuery.pdfOCR.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Tokenizers;

namespace fnf.DocQuery.pdfOCR.Services
{
    public class PdfSummary(IReadPdf readPdf, ILogger<PdfSummary> _logger, IConfiguration configuration) : IPdfSummary
    {
        private readonly IConfiguration configuration = configuration;
        private readonly ILogger<PdfSummary> _logger = _logger;
        private readonly Tokenizer tokenizer = TiktokenTokenizer.CreateForModel(configuration["AzureCred:model"]!);
        private readonly int inputTokensize = Convert.ToInt32(Convert.ToInt32(configuration["AzureCred:modelMaxTokens"]) * 0.25);
        private readonly IReadPdf readPdf = readPdf;
        /// <summary>
        ///     Returns the text for summarization of document
        /// </summary>
        /// <param name="pdfFile">IFormFile pdffile</param>
        /// <returns>string - text read from pdf</returns>
        public string GetSummary(IFormFile pdfFile)
        {
            string text = readPdf.GetText(pdfFile);
            int EndIndex = tokenizer.GetIndexByTokenCount(text, maxTokenCount: inputTokensize, out string? normalizedText, out int count);
            EndIndex = Math.Min(EndIndex, text.Length - 1);
            return text.Substring(0, EndIndex + 1)!;
        }
    }
}
