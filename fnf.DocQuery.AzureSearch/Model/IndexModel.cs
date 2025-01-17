using System.ComponentModel.DataAnnotations;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace fnf.DocQuery.AzureSearch.Model
{

    public class IndexModel
    {
        [SearchableField(AnalyzerName = "standard.lucene",IsKey = true)]
        public string id { get; set; }

        [VectorSearchField(VectorSearchDimensions = 1536, VectorSearchProfileName = "my-vector-profile",IsHidden = true)]
        public List<float> embedding { get; set; }

        [SearchableField(AnalyzerName = "standard.lucene")]
        public List<string> original_text { get; set; }

        [SearchableField(AnalyzerName = "standard.lucene", IsFilterable = true)]
        public string reference { get; set; }

        [SearchableField(AnalyzerName = "standard.lucene", IsFilterable = true)]
        public string fileId { get; set; }
    }
}
