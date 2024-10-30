using MongoDB.Driver;
using ProgEntLib.DTO;
using ProgEntLib.Models;

namespace ProgEntLib.Service
{
    public class CategoriaService
    {
        private readonly IMongoCollection<Categoria> _categoriaCollection;
        private readonly IMongoCollection<Libro> _libroCollection;

        public CategoriaService(IMongoCollection<Categoria> categoriaCollection,
            IMongoCollection<Libro> libroCollection)
        {
            _categoriaCollection = categoriaCollection;
            _libroCollection = libroCollection;
        }

        public async Task<Categoria> CreaCategoriaAsync(DTOCategoria categoria)
        {
            var existingCategoria = await _categoriaCollection.Find
                (c => c.Nome.ToLower() == categoria.Nome).FirstOrDefaultAsync();
            if (existingCategoria == null)
            {
                return null;
            }

            var trueCategoria = new Categoria
            {
                Nome = categoria.Nome
            };

            await _categoriaCollection.InsertOneAsync(trueCategoria);
            return trueCategoria;
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

