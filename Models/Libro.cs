using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProgEntLib.Models
{
    public class Libro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public required string Nome { get; set; }

        [BsonRequired]
        public required string Autore { get; set; }

        [BsonRequired]
        public required DateOnly Data { get; set; }

        [BsonRequired]
        public required string Editore { get; set; }

        public required List<Categoria> Categorie { get; set; }
    }
}
