using MongoDB.Driver;
using ProgEntLib.Models;

namespace ProgEntLib.Service
{
    public class CategoriaService
    {
        private readonly IMongoCollection<Categoria> _categoriaCollection;
        private readonly IMongoCollection<Libro> _libroCollection;
        private readonly ILogger<UtenteService> _logger;

        public CategoriaService(IMongoCollection<Categoria> categoriaCollection,
            IMongoCollection<Libro> libroCollection,
            ILogger<UtenteService> logger)
        {
            _categoriaCollection = categoriaCollection;
            _libroCollection = libroCollection;
            _logger = logger;
        }

        public async Task<Categoria> CreaCategoriaAsync(string categoria)
        {
            var existingCategoria = await _categoriaCollection.Find
                (c => c.Nome == categoria).FirstOrDefaultAsync();
            if (existingCategoria == null)
            {
                var nuovaCategoria = new Categoria
                {
                    Nome = categoria
                };

                await _categoriaCollection.InsertOneAsync(nuovaCategoria);
                return nuovaCategoria;
            }
            
            return null;
        }

        public async Task<bool> CancellaCategoriaAsync(string category)
        {
            var isCategoryUsed = await _libroCollection.Find
            (b => b.Categorie.Any
                (c => c.Nome == category)).AnyAsync();
            if (isCategoryUsed)
                return false;
            var deleteResult = await _categoriaCollection.DeleteOneAsync(c => c.Nome == category);
            return deleteResult.DeletedCount > 0;
        }

        public async Task<List<Categoria>> tutteCategorieAsync()
        {
            return await _categoriaCollection.Find(_ => true).ToListAsync();
        }
    }
}

