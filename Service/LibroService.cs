using MongoDB.Bson;
using MongoDB.Driver;
using ProgEntLib.DTO;
using ProgEntLib.Models;
using ProgEntLib.Properties;

namespace ProgEntLib.Service
{
    public class LibroService
    {
        private readonly IMongoCollection<Libro> _libroCollection;

        public LibroService(IMongoClient mongoClient, MongoDBSettings settings)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _libroCollection = database.GetCollection<Libro>(settings.CollectionNames.Libri);
        }

        internal async Task<bool> AggiornaLibroAsync(string id, DTOLibro dtoLibro)
        {
            var update = Builders<Libro>.Update
                .Set(b => b.Nome, dtoLibro.Nome)
                .Set(b => b.Autore, dtoLibro.Autore)
                .Set(b => b.Data, dtoLibro.DataPubblicazione)
                .Set(b => b.Editore, dtoLibro.Editore)
                .Set(b => b.Categorie, dtoLibro.Categorie);

            var result = await _libroCollection.UpdateOneAsync(b => b.Id.ToString() == id, update);
            return result.ModifiedCount > 0;
        }

        internal async Task<bool> CancellaLibroAsync(string id)
        {
            var result = await _libroCollection.DeleteOneAsync(book => book.Id.ToString() == id);
            return result.DeletedCount > 0;
        }

        internal async Task<List<Libro>> CercaLibriAsync(string? categoria, string? nome, string? dataPubblicazione, string? autore, int page, int pageSize)
        {
            var filterBuilder = Builders<Libro>.Filter;
            var filters = new List<FilterDefinition<Libro>>();

            if (!string.IsNullOrEmpty(categoria))
                filters.Add(filterBuilder.ElemMatch(b => b.Categorie, Builders<Categoria>.Filter.Eq(c => c.Nome, categoria)));
        
            if (!string.IsNullOrEmpty(nome))
                filters.Add(filterBuilder.Regex(b => b.Nome, new MongoDB.Bson.BsonRegularExpression(nome, "i")));

            if (!string.IsNullOrEmpty(dataPubblicazione))
            {
                if (DateOnly.TryParseExact(dataPubblicazione, "yyyy-MM-dd", out DateOnly parsedDate))
                {
                    filters.Add(filterBuilder.Eq(b => b.Data, parsedDate));
                }
                else
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(autore))
                filters.Add(filterBuilder.Regex(b => b.Autore, new MongoDB.Bson.BsonRegularExpression(autore, "i")));

            var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            return await _libroCollection.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        internal async Task<Libro> CreaLibroAsync(DTOLibro newBook)
        {
            var book = new Libro
            {
                Nome = newBook.Nome,
                Autore = newBook.Autore,
                Data = newBook.DataPubblicazione,
                Editore = newBook.Editore,
                Categorie = newBook.Categorie
            };

           await _libroCollection.InsertOneAsync(book);
           return book;
        }

        internal async Task<Libro> GetLibroByIdAsync(string id)
        {
            return await _libroCollection.Find(libro => libro.Id.ToString() == id).FirstOrDefaultAsync();
        }
    }
}
