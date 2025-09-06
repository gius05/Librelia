using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Librelia.Models;

public class PasswordResetToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Email { get; set; }

    public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; } = false;
}