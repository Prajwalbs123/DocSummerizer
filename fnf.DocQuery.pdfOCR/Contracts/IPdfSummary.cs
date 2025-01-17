using Microsoft.AspNetCore.Http;

namespace fnf.DocQuery.pdfOCR.Contracts
{
    public interface IPdfSummary
    {
        string GetSummary(IFormFile pdfFile);
    }
}