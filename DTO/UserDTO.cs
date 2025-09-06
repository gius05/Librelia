using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using Librelia.Models;

namespace Librelia.DTO
{
    public class UserDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("isExternal")]
        public bool External { get; set; }

        [BsonElement("status")]
        [JsonPropertyName("status")]
        public string Status { get; set; } = "not-verified";

        [JsonPropertyName("role")]
        public string? Role { get; set; }

       /*[JsonPropertyName("class")]
        public string? SchoolClass { get; set; }*/
    }
}
