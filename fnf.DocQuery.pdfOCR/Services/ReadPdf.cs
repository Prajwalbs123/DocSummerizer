using fnf.DocQuery.pdfOCR.Contracts;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace fnf.DocQuery.pdfOCR.Services
{
    public class ReadPdf(ILogger<ReadPdf> _logger) : IReadPdf
    {
        private readonly ILogger<ReadPdf> _logger = _logger;


        /// <summary>
        ///		Utilize iText.Kernel.Pdf to read text contents from pdf file.
        /// </summary>
        /// <param name="pdfFile">IFormfile - pdffile</param>
        /// <returns>string text - text contents of pdf</returns>
        public string GetText(IFormFile pdfFile)
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
                _logger.LogError($"Error: {ex.Message}");
            }

            return text;
        }

    }

}
