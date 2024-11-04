using Microsoft.Extensions.Options;
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
        private readonly IMongoCollection<Categoria> _categorieCollection;

        public LibroService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _libroCollection = database.GetCollection<Libro>(settings.Value.CollectionNames.Libri);
            _categorieCollection = database.GetCollection<Categoria>(settings.Value.CollectionNames.Categorie);
        }

        internal async Task<bool> AggiornaLibroAsync(string id, DTOLibro dtoLibro)
        {
            var categorie = await filtraDTOCategorie(dtoLibro.Categorie);

            var update = Builders<Libro>.Update
                .Set(b => b.Nome, dtoLibro.Nome)
                .Set(b => b.Autore, dtoLibro.Autore)
                .Set(b => b.Data, dtoLibro.DataPubblicazione)
                .Set(b => b.Editore, dtoLibro.Editore)
                .Set(b => b.Categorie, categorie);

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
            var categorie = await filtraDTOCategorie(newBook.Categorie);

            var book = new Libro
            {
                Nome = newBook.Nome,
                Autore = newBook.Autore,
                Data = newBook.DataPubblicazione,
                Editore = newBook.Editore,
                Categorie = categorie
            };

           await _libroCollection.InsertOneAsync(book);
           return book;
        }

        internal async Task<Libro> GetLibroByIdAsync(string id)
        {
            return await _libroCollection.Find(libro => libro.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<List<Categoria>> filtraDTOCategorie(List<DTOCategoria> dtoCategorie)
        {
            List<Categoria> categorieFinali = null;

            foreach (var dtoCategoria in dtoCategorie)
            {
                // Cerco la categoria nel database
                var categoriaEsistente = await _categorieCollection
                    .Find(c => c.Nome == dtoCategoria.Nome)
                    .FirstOrDefaultAsync();

                if (categoriaEsistente != null)
                {
                    // Se esiste già, la aggiungo alla lista finale
                    categorieFinali.Add(categoriaEsistente);
                }
                else
                {
                    // Altrimenti, creo una nuova categoria e la aggiungo sia al DB che alla lista finale
                    var nuovaCategoria = new Categoria
                    {
                        Nome = dtoCategoria.Nome
                    };
                    await _categorieCollection.InsertOneAsync(nuovaCategoria);
                    categorieFinali.Add(nuovaCategoria);
                }
            }

            return categorieFinali;
        }
    }
}
