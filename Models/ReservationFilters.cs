using System.Text.Json.Serialization;

namespace Librelia.Models;

public class ReservationFilters
{
    [JsonPropertyName("searching")]
    public string Searching { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("sortBy")]
    public string SortBy { get; set; } = "title";

    [JsonPropertyName("sortDirection")]
    public string SortDirection { get; set; } = "asc";

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = null;

    [JsonPropertyName("endDate")]
    public string EndDate { get; set; } = null;
}