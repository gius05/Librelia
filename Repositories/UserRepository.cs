using Librelia.Database;
using Librelia.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Librelia.Repositories
{
    public class UserRepository : SettingsRepo
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDBContext database)
        {
            // Preleva tutta la collezione
            _users = database.GetCollection<User>("Users");
        }
        
        
        /// <summary>
        /// Return all users in the collection
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<User>> GetAll() =>
            await _users.Find(_ => true).ToListAsync();

        
        /// <summary>
        /// Return amount in the collection
        /// </summary>
        /// <returns></returns>
        public async ValueTask<int> GetCount()
        {
            var users = await _users.Find(_ => true).ToListAsync();

            return users.Count;
        }
        /// <summary>
        /// Return user by email if exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async ValueTask<User> GetByEmail(string email) =>
            await _users.Find(user => user.Email.Equals(email)).FirstOrDefaultAsync();


        /// <summary>
        /// Return user by id if exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<User> GetById(string id) =>
            await _users.Find(user => user.Id.Equals(id)).FirstOrDefaultAsync();

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="newUser"></param>
        public async ValueTask AddUser(User newUser)
        {
            newUser.CreatedAt = DateTime.Now;
            newUser.UpdatedAt = DateTime.Now;
            await _users.InsertOneAsync(newUser);
        }

        /// <summary>
        /// Update user info by giving the all object with the new changes
        /// </summary>
        /// <param name="updatedUserRecord"></param>
        public async ValueTask UpdateUser(User updatedUserRecord)
        {
            updatedUserRecord.UpdatedAt = DateTime.Now;

            var filter = Builders<User>.Filter.Eq("_id", new ObjectId(updatedUserRecord.Id));
            
            var updateDefinition = new BsonDocument("$set", updatedUserRecord.ToBsonDocument());
            updateDefinition.Remove("_id"); // Rimuove _id per evitare errori

            await _users.UpdateOneAsync(filter, updateDefinition);
        }

        /// <summary>
        /// Verify the user
        /// </summary>
        /// <param name="id"></param>
        public async ValueTask VerifyUser(string id)
        {
            var user = await _users.Find(x => x.Id.Equals(id)).FirstAsync();

            user.Status = "verified";
            user.UpdatedAt  = DateTime.Now;

            var filter = Builders<User>.Filter.Eq("_id", new ObjectId(user.Id));

            var updateDefinition = new BsonDocument("$set", user.ToBsonDocument());
            updateDefinition.Remove("_id"); // Rimuove _id per evitare errori

            await _users.UpdateOneAsync(filter, updateDefinition);
        }

        /// <summary>
        /// Remove user by id
        /// </summary>
        /// <param name="id"></param>
        public async ValueTask RemoveUser(string id)
        {
            await _users.DeleteOneAsync(x => x.Id.Equals(id));
        }

        /// <summary>
        /// Search users by email, name, surname, role or verified and sort in asc by deafault or desc if you want
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async ValueTask<List<User>> SearchUsers(UserFilter filters)
        {
            var filterDefinitionBuilder = Builders<User>.Filter;
            var filter = filterDefinitionBuilder.Empty;

            var sortBuilder = Builders<User>.Sort;
            var sort = filters.SortDirection.Trim() == "asc"
                ? sortBuilder.Ascending(filters.SortBy.Trim())
                : sortBuilder.Descending(filters.SortBy.Trim());

            if (!string.IsNullOrEmpty(filters.Searching) && !string.IsNullOrWhiteSpace(filters.Searching))
                filter &= filterDefinitionBuilder.Where(b => b.Name.ToLower().Contains(filters.Searching.Trim())
                                                             || b.Surname.ToLower().Contains(filters.Searching.Trim().ToLower())
                                                             || b.Role.ToLower().Contains(filters.Searching.Trim().ToLower())
                                                             || b.Email.ToLower().Contains(filters.Searching.Trim().ToLower()));

            if (!string.IsNullOrWhiteSpace(filters.Status) && !string.IsNullOrEmpty(filters.Status))
                filter &= filterDefinitionBuilder.Eq(b => b.Status, filters.Status);


            return await _users.Find(filter, _findOptions).Sort(sort).ToListAsync();
        }
        
    }
}
