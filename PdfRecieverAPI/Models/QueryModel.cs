namespace PdfRecieverAPI.Models
{
    public class QueryModel
    {
        public string Message { get; set; } = string.Empty;
        public int NoSentence { get; set; } = 5;
        public string? FileName { get; set; } = string.Empty;
    }
}
