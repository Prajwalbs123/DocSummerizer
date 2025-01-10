using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Tokenizers;

namespace PdfReadChunkDLL
{

	public class PdfReadChunk(IConfiguration configuration, ILogger<PdfReadChunk> _logger) : IPdfReadChunk
	{
		private readonly IConfiguration configuration = configuration;
		private readonly ILogger<PdfReadChunk> _logger = _logger;
		private readonly Tokenizer tokenizer = TiktokenTokenizer.CreateForModel(configuration["AzureCred:model"]!);
		private readonly int inputTokensize = Convert.ToInt32(Convert.ToInt32(configuration["AzureCred:modelMaxTokens"]) * 0.25);

        /// <summary>
        ///		Utilize iText.Kernel.Pdf to read text contents from pdf file.
        /// </summary>
        /// <param name="pdfFile">IFormfile - pdffile</param>
        /// <returns>string text - text contents of pdf</returns>
        private string ReadPdf(IFormFile pdfFile)
		{
			_logger.LogInformation("Reading Pdf");
			string text = "";
			try
			{
				using Stream filestream = pdfFile.OpenReadStream();
				using PdfReader reader = new(filestream);
				using PdfDocument pdfDoc = new(reader);

				for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
				{
					text += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
				}
				_logger.LogInformation("Pdf read");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error: {ex.Message}");
			}

			return text;
		}

		/// <summary>
		///     Returns the text for summarization of document
		/// </summary>
		/// <param name="pdfFile">IFormFile pdffile</param>
		/// <returns>string - text read from pdf</returns>
		public string GetText(IFormFile pdfFile)
		{
			string text = ReadPdf(pdfFile);
			int EndIndex = tokenizer.GetIndexByTokenCount(text, maxTokenCount: inputTokensize, out string? normalizedText, out int count);
			EndIndex = Math.Min(EndIndex, text.Length - 1);
			return text.Substring(0, EndIndex + 1)!;
		}

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
				string text = ReadPdf(file);
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
				_logger.LogError(ex, $"Error: {ex.Message}");
			}

			return chunks;
		}
	}
}
