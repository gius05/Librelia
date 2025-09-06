using System.Text.Json.Serialization;
using Librelia.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Book : BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null;

    [BsonElement("title")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = null;

    [BsonElement("subtitle")]
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; } = null;

    [BsonElement("imagePath")]
    [JsonPropertyName("imagePath")]
    public string ImagePath { get; set; } = null;

    [BsonElement("description")]
    [JsonPropertyName("description")]
    public string? Description { get; set; } = null;

    [BsonElement("isbn")]
    [JsonPropertyName("isbn")]
    public string Isbn { get; set; } = null;

    [BsonElement("categories")]
    [JsonPropertyName("categories")]
    public List<string> Categories { get; set; } = null;

    [BsonElement("house")]
    [JsonPropertyName("house")]
    public string House { get; set; } = null;

    [BsonElement("language")]
    [JsonPropertyName("language")]
    public string Language { get; set; } = null;

    [BsonElement("release")]
    [JsonPropertyName("release")]
    public int? Release { get; set; } = 0;

    [BsonElement("authors")]
    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; } = null;

    [BsonElement("position")]
    [JsonPropertyName("position")]
    public int Position { get; set; } = 0;

    [BsonElement("collocation")]
    [JsonPropertyName("collocation")]
    public string Collocation { get; set; } = null;

    [BsonElement("reserved")]
    [JsonPropertyName("reserved")]
    public bool Reserved { get; set; } = false;

    // Metodo ToString personalizzato
    public override string ToString()
    {
        var authors = Authors != null && Authors.Any() ? string.Join(", ", Authors) : "Unknown authors";
        var categories = Categories != null && Categories.Any() ? string.Join(", ", Categories) : "No categories";
        var releaseYear = Release.HasValue ? Release.Value.ToString() : "Unknown release year";
        var house = string.IsNullOrEmpty(House) ? "No publisher" : House;
        var language = string.IsNullOrEmpty(Language) ? "No language" : Language;

        
        return $"Id: {Id}\n" +
               $"Title: {Title}\n" +
               $"Subtitle: {Subtitle ?? "No subtitle"}\n" +
               $"Authors: {authors}\n" +
               $"ISBN: {Isbn ?? "No ISBN"}\n" +
               $"Categories: {categories}\n" +
               $"Publisher: {house}\n" +
               $"Language: {language}\n" +
               $"Release Year: {releaseYear}\n" +
               $"Description: {Description ?? "No description available"}\n" +
               $"Position: {Position}\n" +
               $"Collocation: {Collocation ?? "No collocation"}\n" +
               $"Reserved: {(Reserved ? "Yes" : "No")}";
    }
    // Metodo ToString personalizzato
    public string ToStringSelect()
    {
        var authors = Authors != null && Authors.Any() ?  Authors.Count <= 2 ?  string.Join(",", Authors) : Authors.First() : "Sconosciuto/i";
        var releaseYear = Release.HasValue ? Release.Value.ToString() : "Sconosciuto";
        var house = string.IsNullOrEmpty(House) ? "Sconosiuto" : House;
        var language = string.IsNullOrEmpty(Language) ? "Sconosciuta" : Language;

        var text = $"Titolo: {Title} " +
               $"Sottotitolo: {Subtitle ?? "No Sottotitolo"} " +
               $"Autore/i: [{authors}] " +
               $"ISBN: {Isbn ?? "No ISBN"} " +            
               $"Editore: {house} " +
               $"Pubblicato: {releaseYear}\n";

        return text.Length > 100 ? text.Substring(0, 100) + "..." : text ;
    }
}
