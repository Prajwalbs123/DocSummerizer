namespace DocQuery.Model
{
    public class QueryModel
    {
        public string Message { get; set; } = string.Empty;
        public int NoSentence { get; set; } = 5;
        public string? FileId { get; set; } 
    }
}
