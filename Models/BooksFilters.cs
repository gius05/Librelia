using System.Text.Json.Serialization;

namespace Librelia.Models
{
    public class BookFilters
    {
        [JsonPropertyName("searching")]
        public string Searching { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("availability")]
        public string Availability { get; set; } = string.Empty;

        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; } = "title";

        [JsonPropertyName("sortDirection")]
        public string SortDirection { get; set; } = "asc";
    }
}
