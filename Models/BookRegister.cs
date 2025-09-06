using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Librelia.Models;

public class BookRegister
{
    [BsonElement("title")]
    [JsonPropertyName("title")]
    [Required]
    public string Title { get; set; } = null;

    [BsonElement("subtitle")]
    [JsonPropertyName("subtitle")]
    [Required]

    public string Subtitle { get; set; } = null;

    [BsonElement("image")]
    [JsonPropertyName("image")]
    [Required]
    public Image Image { get; set; } = null;

    [BsonElement("description")]
    [JsonPropertyName("description")]
    [Required]
    public string Description { get; set; } = null;

    [BsonElement("isbn")]
    [JsonPropertyName("isbn")]
    [Required]
    public string Isbn { get; set; } = null;

    [BsonElement("categories")]
    [JsonPropertyName("categories")]
    [Required]
    public List<string> Categories { get; set; } = null;

    [BsonElement("house")]
    [JsonPropertyName("house")]
    [Required]
    public string House { get; set; } = null;

    [BsonElement("language")]
    [JsonPropertyName("language")]
    [Required]
    public string Language { get; set; } = null;

    [BsonElement("release")]
    [JsonPropertyName("release")]
    [Required]
    public DateTime Release { get; set; } = DateTime.Now;

    [BsonElement("authors")]
    [JsonPropertyName("authors")]
    [Required]
    public List<string> Authors { get; set; } = null;

    [BsonElement("amount")]
    [JsonPropertyName("amount")]
    [Required]
    public int Amount { get; set; } = 0;

    [BsonElement("position")]
    [JsonPropertyName("position")]
    [Required]
    public int Position { get; set; } = 0;

    [BsonElement("collocation")]
    [JsonPropertyName("collocation")]
    [Required]
    public string Collocation { get; set; } = null;
}