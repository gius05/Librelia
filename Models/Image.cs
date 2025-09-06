using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Librelia.Models
{
    public class Image
    {
        [BsonElement("thumbnail")]
        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; set; }
        [BsonElement("smallThumbnail")]
        [JsonPropertyName("smallThumbnail")]
        public string? SmallThumbnail { get; set; }

    }
}
