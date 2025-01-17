using Microsoft.AspNetCore.Http;

namespace fnf.DocQuery.pdfOCR.Contracts
{
    public interface IReadPdf
    {
        string GetText(IFormFile pdfFile);
    }
}