using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Librelia.Models
{
    public class BaseEntity
    {
        [BsonElement("createdAt")]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updatedAt")]
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
