using Librelia.Database;
using Librelia.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Librelia.Repositories
{
    public class BookRepository : SettingsRepo
    {
        private readonly IMongoCollection<Book> _books;
        public BookRepository(MongoDBContext database)
        {
            // Preleva tutta la collezione
            _books = database.GetCollection<Book>("Books");
        }

        /// <summary>
        /// Get all books in the repo
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<Book>> GetAll() =>
         await _books.Find(new BsonDocument()).ToListAsync();
    

        /// <summary>
        /// Get the count of books
        /// </summary>
        /// <returns></returns>
        public async ValueTask<int> GetCount()
        {
            var books = await _books.Find(_ => true).ToListAsync();

            return books.Count;
        } 

        
        /// <summary>
        /// Search and return element with same id of request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<Book> GetById(string id) =>
            await _books.Find(book => book.Id.Equals(id)).FirstOrDefaultAsync();

        /// <summary>
        /// Search and return element with same isbn of request
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public async ValueTask<Book> GetByIsbn(string isbn) =>
            await _books.Find(book => book.Isbn.Equals(isbn)).FirstOrDefaultAsync();
        
        /// <summary>
        /// Check if the book with same id is not reserved
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<Book> GetBookForPrenotation(string id) =>
            await _books.Find(book => book.Id.Equals(id) && book.Reserved == false).FirstOrDefaultAsync();


        /// <summary>
        /// Add new book to repo
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns></returns>
        public async ValueTask AddBook(Book newBook)
        {
            newBook.CreatedAt = DateTime.Now;
            newBook.UpdatedAt = DateTime.Now;
            await _books.InsertOneAsync(newBook);
        }

        /// <summary>
        /// Update the previous book record
        /// </summary>
        /// <param name="updatedBookRecord"></param>
        /// <returns></returns>
        public async ValueTask UpdateBook(Book updatedBookRecord)   // Aggiorna i record dei libri in base all'Id
        {
            updatedBookRecord.UpdatedAt = DateTime.Now;
            var filter = Builders<Book>.Filter.Eq("Id", updatedBookRecord.Id);
            var update = updatedBookRecord.ToBsonDocument();

            await _books.FindOneAndUpdateAsync(filter, update);
        }

        /// <summary>
        /// Remove the book
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        public async ValueTask RemoveBook(string BookId)
        {
            var filter = Builders<Book>.Filter.Eq("Id", $"{BookId}");
            await _books.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Search for a specific book/books
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async ValueTask<List<Book>> SearchBooks(BookFilters filters)
        {
            var filterDefinitionBuilder = Builders<Book>.Filter;
            var filter = filterDefinitionBuilder.Empty;
                
            var sortBuilder = Builders<Book>.Sort;
            var sort = filters.SortDirection.Trim() == "asc"
                ? sortBuilder.Ascending(filters.SortBy.Trim())
                : sortBuilder.Descending(filters.SortBy.Trim());


             if(!string.IsNullOrEmpty(filters.Searching) && !string.IsNullOrWhiteSpace(filters.Searching))
             filter &= filterDefinitionBuilder.Where(b => b.Isbn.Contains(filters.Searching.Trim())
                        || b.Title.ToLower().Contains(filters.Searching.Trim().ToLower())
                        || b.House.ToLower().Contains(filters.Searching.Trim().ToLower())
                        || b.Authors.Any(author => author.ToLower().Contains(filters.Searching.Trim().ToLower())));




             if(!string.IsNullOrEmpty(filters.Availability) && !string.IsNullOrWhiteSpace(filters.Availability))
             if(filters.Availability.Equals("available"))
             {
                 filter &= filterDefinitionBuilder.Where(b => b.Reserved == false);
             } else if (filters.Availability.Equals("unavailable"))
             {
                filter &= filterDefinitionBuilder.Where(b => b.Reserved == true);
             }


            if (!string.IsNullOrEmpty(filters.Category) && !string.IsNullOrWhiteSpace(filters.Category))
            {
                filter &= filterDefinitionBuilder.Where(b => b.Categories.Any(category =>
                 category.ToLower().Contains(filters.Category.Trim().ToLower())));
            }
                
                
            

            return await _books.Find(filter, _findOptions).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Set the book Reserverd property to false
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<bool> BookReturned(string id)
        {
            var book = await _books.Find(b => b.Id == id).FirstAsync();
            if(book is null)
            {
                return false;
            }
            book.Reserved = false;
            
                        
            var filter = Builders<Book>.Filter.Eq("_id", new ObjectId(book.Id));

            var updateDefinition = new BsonDocument("$set", book.ToBsonDocument());
            updateDefinition.Remove("_id"); // Rimuove _id per evitare errori
            
            await _books.UpdateOneAsync(filter, updateDefinition);
            return true;
        }
    }
}

