using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProgEntLib.Models
{
    public class Categoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public required string Nome { get; set; }
    }
}
