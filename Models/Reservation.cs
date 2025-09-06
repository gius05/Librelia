using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Librelia.Models
{
    public class Reservation : BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null;

        [BsonElement("register_date")]
        public DateTime Register_Date { get; set; } = DateTime.Now;

        [BsonElement("expire_date")]
        [JsonPropertyName("expire_date")]
        public DateTime Expire_Date { get; set; } = DateTime.Now;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null;

        [BsonElement("bookId")]
        [JsonPropertyName("bookId")]
        public string BookId { get; set; } = null;
        
        [BsonElement("mailSent_Date")]
        public DateTime MailSent_Date { get; set; } = DateTime.MinValue;
        
        [BsonElement("status")]
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
