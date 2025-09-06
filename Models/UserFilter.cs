using System.Text.Json.Serialization;

namespace Librelia.Models
{

    public class UserFilter
    {
        [JsonPropertyName("searching")] public string Searching { get; set; } = string.Empty;

        [JsonPropertyName("status")] public string? Status { get; set; } = null;

        [JsonPropertyName("sortBy")] public string SortBy { get; set; } = "name";

        [JsonPropertyName("sortDirection")] public string SortDirection { get; set; } = "asc";
    }
}