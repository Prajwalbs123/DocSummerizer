using GptDLL;
using PdfReadChunkDLL;
using PdfRecieverAPI.Contracts;
using StoreEmbeddingsDLL;

namespace PdfRecieverAPI.Services
{
    public class UploadService(IPdfReadChunk pdfchunk, ILogger<UploadService> _logger, IGptCall gptCall, IStoreEmbeddedData storeEmbeddedData) : IUploadService
	{
		private readonly ILogger<UploadService> _logger = _logger;
		private readonly IPdfReadChunk pdfchunk = pdfchunk;
		private readonly IGptCall gptCall = gptCall;
		private readonly IStoreEmbeddedData storeEmbeddedData = storeEmbeddedData;

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
				fileText = pdfchunk.GetText(file);

				response = await gptCall.GptSummarize(fileText, file.FileName);
				_logger.LogInformation("Gpt summarization response processed");

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error: {ex.Message}");
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

				await storeEmbeddedData.Embed(pdfchunk.GetChunk(file), file.FileName);
				_logger.LogInformation($"{file.FileName} Upload Successful");
				return $"{file.FileName} uploaded successfully";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error: {ex.Message}");
				return $"{file.FileName} uploaded failed";
			}
		}
	}
}
