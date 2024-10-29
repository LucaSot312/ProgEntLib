using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProgEntLib.Models
{
    public class Utente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public required string Email { get; set; }

        [BsonRequired]
        public required string Nome { get; set; }

        [BsonRequired]
        public required string Cognome { get; set; }

        [BsonRequired]
        public required string Password { get; set; }
    }
}
