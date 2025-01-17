using System.ComponentModel.DataAnnotations;

namespace fnf.DocQuery.API.Models
{
    public class QueryModel
    {
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; } = string.Empty;
        [Range(1, 10, ErrorMessage = "NoSentence must be between 1 and 10")]
        public int NoSentence { get; set; } = 5;
        public string? FileId { get; set; } = null;
    }
}
