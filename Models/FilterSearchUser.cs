using System.Text.Json.Serialization;

namespace Librelia.Models
{
    public class FilterSearchUser
    {
        [JsonPropertyName("searching")]
        public string Searching { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("admin")]
        public string Admin { get; set; } = string.Empty;

        [JsonPropertyName("verified")]
        public string Verified { get; set; } = string.Empty;

        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; } = "name";

        [JsonPropertyName("sortDirection")]
        public string SortDirection { get; set; } = "asc";
    }
}
