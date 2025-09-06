using Librelia.Database;
using Librelia.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Librelia.Repositories;

public class ResetTokensRepository : SettingsRepo
{
    public IMongoCollection<PasswordResetToken> _resetTokens;
    public ResetTokensRepository(MongoDBContext database)
    {
        // Preleva tutta la collezione
        _resetTokens = database.GetCollection<PasswordResetToken>("Reset-Password");
    }
    
    /// <summary>
    /// Add a new token for the reset password link
    /// </summary>
    /// <param name="newData"></param>
    /// <returns></returns>
    public async ValueTask Add(PasswordResetToken newData) =>
        await _resetTokens.InsertOneAsync(newData);
    
    /// <summary>
    /// Update the status of the record to exipired or used
    /// </summary>
    /// <param name="updateRecord"></param>
    /// <returns></returns>
    public async ValueTask Update(PasswordResetToken updateRecord) 
    {
        var filter = Builders<PasswordResetToken>.Filter.Eq("Id", updateRecord.Id);
        var update = updateRecord.ToBsonDocument();

        await _resetTokens.FindOneAndUpdateAsync(filter, update);
    }
    /// <summary>
    /// Return the record by email && token if is not used or expired
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<PasswordResetToken> GetByEmailAndToken(string email, string token) =>
        await _resetTokens.Find(t =>
            t.Token == token &&
            t.Email == email &&
            t.ExpiresAt > DateTime.UtcNow &&
            !t.Used
        ).FirstOrDefaultAsync();
    
}