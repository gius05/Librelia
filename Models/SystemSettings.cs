using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Librelia.Models;

public class SystemSettings
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null;
    
    [BsonElement("itemsForPage")]
    [JsonPropertyName("itemsForPage")]
    public int ItemsForPage { get; set; } = 20;

    [BsonElement("bookForUser")]
    [JsonPropertyName("bookForUser")]
    public string BookForUser { get; set; } = null;
}