using Microsoft.AspNetCore.Http;

namespace PdfReadChunkDLL
{
	public interface IPdfReadChunk
	{
		List<string> GetChunk(IFormFile file);
		string GetText(IFormFile pdfFile);
	}
}