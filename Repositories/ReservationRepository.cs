using Librelia.Database;
using Librelia.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Librelia.Repositories
{
    public class ReservationRepository : SettingsRepo
    {
        private readonly IMongoCollection<Reservation> _reservations;

        public ReservationRepository(MongoDBContext database)
        {
            // Preleva tutta la collezione
            _reservations = database.GetCollection<Reservation>("Reservations");
        }

        public async ValueTask<List<Reservation>> GetAll() =>
            await _reservations.Find(_ => true).ToListAsync();

        public async ValueTask<int> GetCount()
        {
            var res = await _reservations.Find(_ => true).ToListAsync();

            return res.Count;
        } 
        

        public async ValueTask<List<Reservation>> GetByEmailOrBookId(string email, string bookId, string state)
        {
            if (email is null) return await _reservations.Find(reservation => reservation.BookId.Equals(bookId) && reservation.Status.Equals(state, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            if (bookId is null) return await _reservations.Find(reservation => reservation.Email.Equals(email) && reservation.Status.Equals(state, StringComparison.OrdinalIgnoreCase)).ToListAsync();

            return await _reservations.Find(reservation => reservation.Email.Equals(email) && reservation.BookId.Equals(bookId) && reservation.Status.Equals(state, StringComparison.OrdinalIgnoreCase)).ToListAsync();
        }

        /// <summary>
        /// Remove reservation by id
        /// </summary>
        /// <param name="id"></param>
        public async ValueTask RemoveReservation(string id)
        {
            await _reservations.DeleteOneAsync(x => x.Id.Equals(id));
        }

        /// <summary>
        /// Return reservation by id if exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<Reservation> GetById(string id) =>
            await _reservations.Find(res => res.Id.Equals(id)).FirstOrDefaultAsync();

        public async ValueTask AddReservation(Reservation newReservation)
        {
            newReservation.CreatedAt = DateTime.Now;
            newReservation.UpdatedAt = DateTime.Now;

            await _reservations.InsertOneAsync(newReservation);
        }

        public async ValueTask UpdateReservation(Reservation updatedReservationRecord)  // Aggiorna i record delle prenotazioni in base all'Id
        {
            updatedReservationRecord.UpdatedAt = DateTime.Now;

            updatedReservationRecord.Expire_Date = updatedReservationRecord.Expire_Date.Date.AddDays(1).AddTicks(-1); // Setta il tempo a 23:59:59.999;
            
            
            var filter = Builders<Reservation>.Filter.Eq("_id", new ObjectId(updatedReservationRecord.Id));

            var updateDefinition = new BsonDocument("$set", updatedReservationRecord.ToBsonDocument());
            updateDefinition.Remove("_id"); // Rimuove _id per evitare errori

            await _reservations.FindOneAndUpdateAsync(filter, updateDefinition);

        }

        public async ValueTask<Reservation> GetReservationByID(string id) =>
            await _reservations.Find(r => r.Id.Equals(id)).FirstOrDefaultAsync();
        
        public async ValueTask<List<Reservation>> SearchReservations(string email, ReservationFilters filters)
        {
            var filterDefinitionBuilder = Builders<Reservation>.Filter;
            var filter = filterDefinitionBuilder.Empty;
            
            var sortBuilder = Builders<Reservation>.Sort;
            var sort = filters.SortDirection.Trim() == "asc"
                ? sortBuilder.Ascending(filters.SortBy.Trim())
                : sortBuilder.Descending(filters.SortBy.Trim());
            
            if(email != null)
            filter &= filterDefinitionBuilder.Eq(b => b.Email, email);
            

            if(!string.IsNullOrEmpty(filters.Searching) && !string.IsNullOrWhiteSpace(filters.Searching))
                filter &= filterDefinitionBuilder.Where(b => b.Email.ToLower().Contains(filters.Searching.Trim().ToLower()));



            if (!string.IsNullOrEmpty(filters.Status) && !string.IsNullOrWhiteSpace(filters.Status))
                filter &= filterDefinitionBuilder.Eq(b => b.Status, filters.Status);

            if (DateTime.TryParse(filters.StartDate, out var startDate) && DateTime.TryParse(filters.EndDate, out var endDate))
            {
                endDate = endDate.Date.AddDays(1).AddTicks(-1); // Setta il tempo a 23:59:59.999
                filter &= filterDefinitionBuilder.Where(b => b.Register_Date >= startDate.ToUniversalTime() && b.Expire_Date <= endDate.ToUniversalTime());
            }
            else if (DateTime.TryParse(filters.StartDate, out var forDate))
            {
                filter &= filterDefinitionBuilder.Where(b => b.Register_Date >= forDate.ToUniversalTime());
            }
            else if (DateTime.TryParse(filters.EndDate, out var minorDate))
            {
                minorDate = minorDate.Date.AddDays(1).AddTicks(-1); // Setta il tempo a 23:59:59.999
                filter &= filterDefinitionBuilder.Where(b => b.Expire_Date <= minorDate.ToUniversalTime());
            }



            return await _reservations.Find(filter, _findOptions).Sort(sort).ToListAsync();

        }
    }
}
