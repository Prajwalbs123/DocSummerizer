using Microsoft.AspNetCore.Http;

namespace fnf.DocQuery.pdfOCR.Contracts
{
    public interface IChunk
    {
        List<string> GetChunk(IFormFile file);
    }
}