using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using Librelia.Models;

namespace Librelia.DTO
{
    public class BookDTO 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("subtitle")]
        public String Subtitle { get; set; } = null;

        [JsonPropertyName("image")]
        public Image Image { get; set; } = null;

        [JsonPropertyName("isbn")]
        public string? Isbn { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("categories")]
        public List<string>? Categories { get; set; }

        [JsonPropertyName("authors")]
        public List<string>? Authors { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("house")]
        public string? House { get; set; }

        [JsonPropertyName("release")]
        public DateTime? Release { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; } = 0;

        [JsonPropertyName("position")]
        public int Position { get; set; } = 0;

        [JsonPropertyName("collocation")]
        public string Collocation { get; set; } = null;

    }
}
