using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Librelia.Models
{
    
    public partial class User : BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null;

        [BsonElement("surname")]
        [JsonPropertyName("surname")]
        public string Surname { get; set; } = null;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null;

        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = null;

        [BsonElement("isExternal")]
        [JsonPropertyName("isExternal")]
        public bool External { get; set; } = false;

        [BsonElement("status")]
        [JsonPropertyName("status")]
        public string Status { get; set; } = "not-verified";

        // [Personale ATA, Docente, Studente, Admin]
        [BsonElement("role")]
        [JsonPropertyName("role")]
        public string Role { get; set; } = null;
/*
        [BsonElement("class")]
        [JsonPropertyName("class")]
        public String SchoolClass { get; set; } = null;*/
    }
}
