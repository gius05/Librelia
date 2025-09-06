using Librelia.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Librelia.Database
{
    public class MongoDBContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBSettings> settings)
        {
            // Inizializzazione 
            _client = new MongoClient(settings.Value.ConnectionString);
            _database = _client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            // Preleva le collezioni 
            return _database.GetCollection<T>(collectionName);
        }
    }
}
